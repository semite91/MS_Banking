using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;
using System.Linq.Expressions;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentRepository : IRepository<PaymentEntity>
    {
        private readonly PaymentDbContext _db;
        public PaymentRepository(PaymentDbContext db) => _db = db;

        public async Task AddAsync(PaymentEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Payments.AddAsync(entity, cancellationToken);
        }

        public async Task<PaymentEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<PaymentEntity>> FindAsync(Expression<Func<PaymentEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Payments.Where(predicate).ToListAsync(cancellationToken);
        }

        public void Remove(PaymentEntity entity) => _db.Payments.Remove(entity);

        public void Update(PaymentEntity entity) => _db.Payments.Update(entity);
    }
}
