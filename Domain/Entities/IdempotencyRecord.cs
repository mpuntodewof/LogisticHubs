using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    /// <summary>
    /// Records the outcome of an idempotent write so that a retried request
    /// with the same Idempotency-Key returns the original response instead
    /// of executing the operation again.
    ///
    /// Composite identity is (TenantId, IdempotencyKey) — never just the key
    /// — so two tenants cannot collide on a shared key value.
    /// </summary>
    public class IdempotencyRecord : ITenantScoped
    {
        /// <summary>
        /// Tenant scope. Required. Anonymous endpoints cannot use idempotency
        /// because there is no tenant to scope the key by.
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Client-supplied <c>Idempotency-Key</c> header value.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// HTTP method + path that the original request targeted, e.g.
        /// <c>POST /api/v1/invoices/{id}/issue</c>. Used to reject reuse of
        /// a key against a different endpoint.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// "InProgress" while the original request is still executing,
        /// "Completed" once the response has been captured.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = IdempotencyStatus.InProgress;

        /// <summary>
        /// HTTP status code of the captured response. Only valid when
        /// <see cref="Status"/> is "Completed".
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Captured response body. Only valid when <see cref="Status"/> is
        /// "Completed".
        /// </summary>
        public string? ResponseBody { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }
    }

    public static class IdempotencyStatus
    {
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
    }
}
