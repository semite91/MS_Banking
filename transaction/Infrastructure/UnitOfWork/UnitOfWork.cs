using Transaction.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;

namespace Transaction.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TransactionDbContext _db;
        public UnitOfWork(TransactionDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
