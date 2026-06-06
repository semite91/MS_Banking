using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Entities;
using Transaction.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;
using System.Linq.Expressions;
using TransactionEntity = Transaction.Domain.Entities.Transaction;

namespace Transaction.Infrastructure.Repositories
{
    public class TransactionRepository : IRepository<TransactionEntity>
    {
        private readonly TransactionDbContext _db;
        public TransactionRepository(TransactionDbContext db) => _db = db;

        public async Task AddAsync(TransactionEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Transactions.AddAsync(entity, cancellationToken);
        }

        public async Task<TransactionEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TransactionEntity>> FindAsync(Expression<Func<TransactionEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Transactions.Where(predicate).ToListAsync(cancellationToken);
        }

        public void Remove(TransactionEntity entity) => _db.Transactions.Remove(entity);

        public void Update(TransactionEntity entity) => _db.Transactions.Update(entity);
    }
}
