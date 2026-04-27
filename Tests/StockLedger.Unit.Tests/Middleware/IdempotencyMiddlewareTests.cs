using System.Text;
using API.Filters;
using API.Middleware;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace StockLedger.Unit.Tests.Middleware;

/// <summary>
/// Unit tests for the rewritten <see cref="IdempotencyMiddleware"/>.
/// Each test uses a fresh in-memory EF context so state never bleeds.
/// </summary>
public class IdempotencyMiddlewareTests
{
    private static readonly Guid TenantA = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111");
    private static readonly Guid TenantB = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222");

    private static AppDbContext CreateDbContext(string? dbName = null, Guid? tenantForFilter = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(
                Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        // The global query filter on IdempotencyRecord references the
        // _tenantContext private field on AppDbContext. The InMemory provider
        // chokes on a null-field reference when it tries to translate the
        // filter expression, so always inject a non-null ITenantContext —
        // pass null TenantId to load *all* rows (the filter short-circuits
        // to true when TenantId is null).
        var ctx = Substitute.For<ITenantContext>();
        ctx.TenantId.Returns(tenantForFilter);
        return new AppDbContext(options, ctx);
    }

    private static ITenantContext TenantContext(Guid? tenantId)
    {
        var ctx = Substitute.For<ITenantContext>();
        ctx.TenantId.Returns(tenantId);
        return ctx;
    }

    private static HttpContext BuildRequest(
        string method,
        string path,
        string? idempotencyKey = null,
        bool requireIdempotency = false)
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Method = method;
        ctx.Request.Path = path;
        if (idempotencyKey != null)
            ctx.Request.Headers["Idempotency-Key"] = idempotencyKey;

        ctx.Response.Body = new MemoryStream();

        if (requireIdempotency)
        {
            // Attach the [Idempotent] attribute via endpoint metadata so the
            // middleware sees it via context.GetEndpoint().Metadata.
            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new IdempotentAttribute()),
                displayName: "test-endpoint");
            ctx.SetEndpoint(endpoint);
        }
        return ctx;
    }

    [Fact]
    public async Task NonWriteMethod_PassesThrough()
    {
        var db = CreateDbContext();
        var ctx = BuildRequest("GET", "/api/v1/anything");
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(_ => { nextCalled++; return Task.CompletedTask; });

        await sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        nextCalled.Should().Be(1);
    }

    [Fact]
    public async Task WriteMethod_NoKey_NotIdempotentRequired_PassesThrough()
    {
        var db = CreateDbContext();
        var ctx = BuildRequest("POST", "/api/v1/anything"); // no key, no [Idempotent]
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(_ => { nextCalled++; return Task.CompletedTask; });

        await sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        nextCalled.Should().Be(1);
        ctx.Response.StatusCode.Should().NotBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task WriteMethod_NoKey_IdempotentRequired_Returns400()
    {
        var db = CreateDbContext();
        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue",
            idempotencyKey: null, requireIdempotency: true);
        var sut = new IdempotencyMiddleware(_ => Task.CompletedTask);

        await sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Key_Without_TenantContext_Returns400()
    {
        var db = CreateDbContext();
        var ctx = BuildRequest("POST", "/api/v1/anything", idempotencyKey: "abc-123");
        var sut = new IdempotencyMiddleware(_ => Task.CompletedTask);

        await sut.InvokeAsync(ctx, db, TenantContext(null));

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FirstRequest_Records_InProgress_Then_Completed()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);
        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue", idempotencyKey: "k1");
        var sut = new IdempotencyMiddleware(async http =>
        {
            http.Response.StatusCode = 200;
            await http.Response.WriteAsync("{\"ok\":true}");
        });

        await sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        ctx.Response.StatusCode.Should().Be(200);

        // Open a fresh context to read what was persisted, bypassing the
        // tenant query filter (no ITenantContext attached).
        using var verify = CreateDbContext(dbName);
        var record = await verify.IdempotencyRecords.SingleAsync();
        record.TenantId.Should().Be(TenantA);
        record.IdempotencyKey.Should().Be("k1");
        record.Status.Should().Be(IdempotencyStatus.Completed);
        record.StatusCode.Should().Be(200);
        record.Endpoint.Should().Be("POST /api/v1/invoices/x/issue");
        record.ResponseBody.Should().Contain("\"ok\":true");
    }

    [Fact]
    public async Task ReplaySameKey_ReturnsCachedResponse_WithoutCallingNext()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);

        // Seed a Completed record
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            TenantId = TenantA,
            IdempotencyKey = "k2",
            Endpoint = "POST /api/v1/invoices/x/issue",
            Status = IdempotencyStatus.Completed,
            StatusCode = 201,
            ResponseBody = "{\"id\":42}",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await db.SaveChangesAsync();

        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue", idempotencyKey: "k2");
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(_ => { nextCalled++; return Task.CompletedTask; });

        // Use a fresh context so the seeded record is loaded via query (not tracking)
        using var freshDb = CreateDbContext(dbName, tenantForFilter: TenantA);
        await sut.InvokeAsync(ctx, freshDb, TenantContext(TenantA));

        nextCalled.Should().Be(0);
        ctx.Response.StatusCode.Should().Be(201);

        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        body.Should().Contain("\"id\":42");
    }

    [Fact]
    public async Task SameKey_DifferentEndpoint_Returns422()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            TenantId = TenantA,
            IdempotencyKey = "k3",
            Endpoint = "POST /api/v1/invoices/x/issue",
            Status = IdempotencyStatus.Completed,
            StatusCode = 200,
            ResponseBody = "{}",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await db.SaveChangesAsync();

        var ctx = BuildRequest("POST", "/api/v1/invoices/x/cancel", idempotencyKey: "k3");
        var sut = new IdempotencyMiddleware(_ => Task.CompletedTask);

        using var freshDb = CreateDbContext(dbName, tenantForFilter: TenantA);
        await sut.InvokeAsync(ctx, freshDb, TenantContext(TenantA));

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task SameKey_WhileInProgress_Returns409()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            TenantId = TenantA,
            IdempotencyKey = "k4",
            Endpoint = "POST /api/v1/invoices/x/issue",
            Status = IdempotencyStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await db.SaveChangesAsync();

        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue", idempotencyKey: "k4");
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(_ => { nextCalled++; return Task.CompletedTask; });

        using var freshDb = CreateDbContext(dbName, tenantForFilter: TenantA);
        await sut.InvokeAsync(ctx, freshDb, TenantContext(TenantA));

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        nextCalled.Should().Be(0);
    }

    [Fact]
    public async Task SameKey_DifferentTenants_DoNotCollide()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);

        // Tenant A stores a Completed record with key "shared"
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            TenantId = TenantA,
            IdempotencyKey = "shared",
            Endpoint = "POST /api/v1/invoices/x/issue",
            Status = IdempotencyStatus.Completed,
            StatusCode = 200,
            ResponseBody = "{\"tenant\":\"A\"}",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await db.SaveChangesAsync();

        // Tenant B sends a request with the same key — must execute, not
        // return tenant A's response.
        var ctx = BuildRequest("POST", "/api/v1/invoices/y/issue", idempotencyKey: "shared");
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(async http =>
        {
            nextCalled++;
            http.Response.StatusCode = 200;
            await http.Response.WriteAsync("{\"tenant\":\"B\"}");
        });

        using var freshDb = CreateDbContext(dbName, tenantForFilter: TenantB);
        await sut.InvokeAsync(ctx, freshDb, TenantContext(TenantB));

        nextCalled.Should().Be(1, "Tenant B request must execute, not be served Tenant A's cache");
        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        body.Should().Contain("\"tenant\":\"B\"");
    }

    [Fact]
    public async Task FailedHandler_RemovesInProgressRow_AllowingRetry()
    {
        var dbName = Guid.NewGuid().ToString();
        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue", idempotencyKey: "k5");
        var sut = new IdempotencyMiddleware(_ =>
            throw new InvalidOperationException("Simulated handler failure"));

        using var db = CreateDbContext(dbName);
        Func<Task> act = () => sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        await act.Should().ThrowAsync<InvalidOperationException>();

        // The InProgress row must have been removed so the client can retry.
        using var verify = CreateDbContext(dbName);
        var any = await verify.IdempotencyRecords.AnyAsync();
        any.Should().BeFalse("failed handler must clean up its InProgress row");
    }

    [Fact]
    public async Task ExpiredRecord_IsDroppedAndRequestProceeds()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = CreateDbContext(dbName);
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            TenantId = TenantA,
            IdempotencyKey = "old",
            Endpoint = "POST /api/v1/invoices/x/issue",
            Status = IdempotencyStatus.Completed,
            StatusCode = 200,
            ResponseBody = "{}",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            ExpiresAt = DateTime.UtcNow.AddDays(-1) // expired
        });
        await db.SaveChangesAsync();

        var ctx = BuildRequest("POST", "/api/v1/invoices/x/issue", idempotencyKey: "old");
        var nextCalled = 0;
        var sut = new IdempotencyMiddleware(http =>
        {
            nextCalled++;
            http.Response.StatusCode = 200;
            return Task.CompletedTask;
        });

        using var freshDb = CreateDbContext(dbName, tenantForFilter: TenantA);
        await sut.InvokeAsync(ctx, freshDb, TenantContext(TenantA));

        nextCalled.Should().Be(1, "expired records should be ignored and the request executed");
    }

    [Fact]
    public async Task OverlongKey_Returns400()
    {
        var db = CreateDbContext();
        var longKey = new string('x', 101);
        var ctx = BuildRequest("POST", "/api/v1/anything", idempotencyKey: longKey);
        var sut = new IdempotencyMiddleware(_ => Task.CompletedTask);

        await sut.InvokeAsync(ctx, db, TenantContext(TenantA));

        ctx.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
