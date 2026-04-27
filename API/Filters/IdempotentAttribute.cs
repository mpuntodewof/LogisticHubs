namespace API.Filters
{
    /// <summary>
    /// Marks an endpoint as REQUIRING the <c>Idempotency-Key</c> header.
    /// <see cref="API.Middleware.IdempotencyMiddleware"/> rejects requests
    /// without the header with HTTP 400 when this attribute is present.
    ///
    /// Apply to financial writes where a duplicate execution would corrupt
    /// state: invoice issue/pay/cancel, payment record, journal entry post,
    /// stock movement, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class IdempotentAttribute : Attribute
    {
    }
}
