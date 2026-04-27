using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Middleware
{
    /// <summary>
    /// Implements safe idempotency for retried writes.
    ///
    /// Contract:
    /// - Only POST/PUT/PATCH requests are inspected.
    /// - Clients opt in via the <c>Idempotency-Key</c> header (free-form, ≤100 chars).
    /// - Records are scoped by <c>(TenantId, IdempotencyKey)</c> — two tenants
    ///   cannot collide on the same key.
    /// - Reusing a key against a *different* endpoint returns 422.
    /// - A duplicate request that arrives while the original is still in flight
    ///   returns 409 (the InProgress row blocks via the unique key).
    /// - Once the original completes, retries return the cached status code +
    ///   response body (TTL = 24h).
    /// - If the original throws, the InProgress row is removed so the client
    ///   can retry safely.
    ///
    /// Endpoints decorated with <see cref="API.Filters.IdempotentAttribute"/>
    /// REQUIRE the header — missing key returns 400. Other endpoints accept
    /// the header but treat it as optional.
    /// </summary>
    public class IdempotencyMiddleware
    {
        private const string IdempotencyKeyHeader = "Idempotency-Key";
        private const int MaxKeyLength = 100;
        private static readonly HashSet<string> _httpMethodsToCheck = new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH" };
        private static readonly TimeSpan RecordTtl = TimeSpan.FromHours(24);

        private readonly RequestDelegate _next;

        public IdempotencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, ITenantContext tenantContext)
        {
            if (!_httpMethodsToCheck.Contains(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();
            var requiresKey = endpoint?.Metadata.GetMetadata<API.Filters.IdempotentAttribute>() != null;
            var keyHeader = context.Request.Headers[IdempotencyKeyHeader].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(keyHeader))
            {
                if (requiresKey)
                {
                    await WriteProblem(context, StatusCodes.Status400BadRequest,
                        "Missing Idempotency-Key header.",
                        "This endpoint requires an Idempotency-Key request header.");
                    return;
                }
                await _next(context);
                return;
            }

            if (keyHeader.Length > MaxKeyLength)
            {
                await WriteProblem(context, StatusCodes.Status400BadRequest,
                    "Invalid Idempotency-Key.",
                    $"Idempotency-Key must be {MaxKeyLength} characters or fewer.");
                return;
            }

            // Idempotency requires a tenant scope. Anonymous endpoints cannot
            // use this middleware safely; reject explicitly.
            if (tenantContext.TenantId is not Guid tenantId)
            {
                await WriteProblem(context, StatusCodes.Status400BadRequest,
                    "Idempotency requires authenticated tenant.",
                    "Idempotency-Key cannot be used on anonymous endpoints.");
                return;
            }

            var endpointSignature = $"{context.Request.Method} {context.Request.Path}";
            if (endpointSignature.Length > 200) endpointSignature = endpointSignature[..200];

            // Look up an existing record (scoped to this tenant via the global
            // query filter on IdempotencyRecord).
            var existing = await dbContext.IdempotencyRecords
                .FirstOrDefaultAsync(r => r.IdempotencyKey == keyHeader);

            if (existing != null && existing.ExpiresAt <= DateTime.UtcNow)
            {
                // Stale — drop and treat as new
                dbContext.IdempotencyRecords.Remove(existing);
                await dbContext.SaveChangesAsync();
                existing = null;
            }

            if (existing != null)
            {
                if (existing.Endpoint != endpointSignature)
                {
                    await WriteProblem(context, StatusCodes.Status422UnprocessableEntity,
                        "Idempotency-Key reused on different endpoint.",
                        $"This key was previously used for '{existing.Endpoint}'.");
                    return;
                }

                if (existing.Status == IdempotencyStatus.InProgress)
                {
                    await WriteProblem(context, StatusCodes.Status409Conflict,
                        "Original request still in flight.",
                        "A request with this Idempotency-Key is already being processed.");
                    return;
                }

                // Completed — replay the cached response.
                context.Response.StatusCode = existing.StatusCode ?? StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                if (!string.IsNullOrEmpty(existing.ResponseBody))
                    await context.Response.WriteAsync(existing.ResponseBody);
                return;
            }

            // No prior record — claim the slot with an InProgress row. The
            // unique (TenantId, IdempotencyKey) PK guarantees that exactly one
            // concurrent request wins; the loser sees a DbUpdateException.
            var record = new IdempotencyRecord
            {
                TenantId = tenantId,
                IdempotencyKey = keyHeader,
                Endpoint = endpointSignature,
                Status = IdempotencyStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(RecordTtl)
            };
            dbContext.IdempotencyRecords.Add(record);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Concurrent duplicate beat us to the insert.
                await WriteProblem(context, StatusCodes.Status409Conflict,
                    "Original request still in flight.",
                    "A request with this Idempotency-Key is already being processed.");
                return;
            }

            // Capture response body so we can persist it on success.
            var originalBodyStream = context.Response.Body;
            using var captureStream = new MemoryStream();
            context.Response.Body = captureStream;

            try
            {
                await _next(context);
            }
            catch
            {
                // The handler threw — remove the InProgress row so the client
                // can retry. Then rethrow so the global exception filter handles
                // the response.
                context.Response.Body = originalBodyStream;
                dbContext.IdempotencyRecords.Remove(record);
                try { await dbContext.SaveChangesAsync(); }
                catch { /* best-effort cleanup */ }
                throw;
            }

            captureStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(captureStream).ReadToEndAsync();
            captureStream.Seek(0, SeekOrigin.Begin);
            await captureStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            // Persist the captured response. Don't cache 5xx — those should be
            // safely retryable by the client.
            var statusCode = context.Response.StatusCode;
            if (statusCode >= 500)
            {
                dbContext.IdempotencyRecords.Remove(record);
            }
            else
            {
                record.Status = IdempotencyStatus.Completed;
                record.StatusCode = statusCode;
                record.ResponseBody = responseBody;
            }
            try { await dbContext.SaveChangesAsync(); }
            catch { /* best-effort: if we crash here the row stays InProgress until TTL */ }
        }

        private static async Task WriteProblem(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new
            {
                type = $"https://httpstatuses.com/{statusCode}",
                title,
                status = statusCode,
                detail
            });
        }
    }
}
