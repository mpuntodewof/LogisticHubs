using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Services
{
    public class TransactionManager : ITransactionManager
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public TransactionManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already in progress.");

            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit.");

            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction == null) return;

            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
