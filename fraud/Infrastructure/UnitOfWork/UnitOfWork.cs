using Fraud.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;

namespace Fraud.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FraudDbContext _db;
        public UnitOfWork(FraudDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
