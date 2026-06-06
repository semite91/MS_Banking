using Fraud.Domain.Entities;
using Fraud.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fraud.Infrastructure.Repositories
{
    public class FraudAlertRepository : IRepository<FraudAlert>
    {
        private readonly FraudDbContext _db;
        public FraudAlertRepository(FraudDbContext db) => _db = db;

        public async Task AddAsync(FraudAlert entity, CancellationToken cancellationToken = default)
        {
            await _db.FraudAlerts.AddAsync(entity, cancellationToken);
        }

        public async Task<FraudAlert?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.FraudAlerts.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<FraudAlert>> FindAsync(Expression<Func<FraudAlert, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.FraudAlerts.Where(predicate).ToListAsync(cancellationToken);
        }

        public void Remove(FraudAlert entity) => _db.FraudAlerts.Remove(entity);

        public void Update(FraudAlert entity) => _db.FraudAlerts.Update(entity);
    }
}
