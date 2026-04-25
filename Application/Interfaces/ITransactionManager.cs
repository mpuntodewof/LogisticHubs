namespace Application.Interfaces
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);

        // Retry-safe wrapper: runs `work` inside a transaction managed by the
        // provider's execution strategy. Required when EnableRetryOnFailure is on.
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct = default);
    }

    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
