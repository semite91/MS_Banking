using Account.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;

namespace Account.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccountDbContext _db;
        public UnitOfWork(AccountDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
