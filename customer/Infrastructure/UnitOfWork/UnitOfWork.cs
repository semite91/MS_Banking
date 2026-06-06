using Customer.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;

namespace Customer.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CustomerDbContext _db;
        public UnitOfWork(CustomerDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
