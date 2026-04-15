namespace Application.Interfaces
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }

    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
