using Account.Domain.Entities;
using Account.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Infrastructure.Repositories
{
    public class AccountRepository : IRepository<AccountEntity>
    {
        private readonly AccountDbContext _db;
        public AccountRepository(AccountDbContext db) => _db = db;

        public async Task AddAsync(AccountEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Accounts.AddAsync(entity, cancellationToken);
        }

        public async Task<AccountEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Accounts.Include(a => a.SubAccounts).FirstOrDefaultAsync(a => a.AccountId == id, cancellationToken);
        }

        public async Task<IEnumerable<AccountEntity>> FindAsync(Expression<Func<AccountEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Accounts.Where(predicate).ToListAsync(cancellationToken);
        }
        public void Remove(AccountEntity entity) => _db.Accounts.Remove(entity);

        public void Update(AccountEntity entity) => _db.Accounts.Update(entity);
    }
}
