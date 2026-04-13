using System.Text.Json;
using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly HashSet<string> _httpMethodsToCheck = new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH" };
        private const string IdempotencyKeyHeader = "Idempotency-Key";

        public IdempotencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            if (!_httpMethodsToCheck.Contains(context.Request.Method))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var keyValues)
                || string.IsNullOrWhiteSpace(keyValues.FirstOrDefault()))
            {
                // No idempotency key — proceed normally
                await _next(context);
                return;
            }

            var idempotencyKey = keyValues.First()!;
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            // Check for existing record
            var existing = await dbContext.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey);

            if (existing != null && existing.ExpiresAt > DateTime.UtcNow)
            {
                // Return cached response
                context.Response.StatusCode = existing.StatusCode;
                context.Response.ContentType = "application/json";
                if (!string.IsNullOrEmpty(existing.ResponseBody))
                {
                    await context.Response.WriteAsync(existing.ResponseBody);
                }
                return;
            }

            // Capture the original response body
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            // Read what was written
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            // Save idempotency record (only for success or conflict responses)
            var statusCode = context.Response.StatusCode;
            if (statusCode >= 200 && statusCode < 500)
            {
                var record = new IdempotencyRecord
                {
                    IdempotencyKey = idempotencyKey,
                    Endpoint = endpoint.Length > 50 ? endpoint[..50] : endpoint,
                    StatusCode = statusCode,
                    ResponseBody = responseBody,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                try
                {
                    if (existing != null)
                    {
                        dbContext.IdempotencyRecords.Update(record);
                    }
                    else
                    {
                        dbContext.IdempotencyRecords.Add(record);
                    }
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Race condition — another request saved first. That's fine.
                }
            }

            // Copy response back to original stream
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}
