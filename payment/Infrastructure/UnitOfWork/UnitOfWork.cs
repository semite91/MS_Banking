using Payment.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;

namespace Payment.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _db;
        public UnitOfWork(PaymentDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
