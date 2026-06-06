using Customer.Domain.Entities;
using Customer.Infrastructure.Persistence;
using Shared.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using CustomerEntity = Customer.Domain.Entities.Customer;
using System.Linq.Expressions;

namespace Customer.Infrastructure.Repositories
{
    public class CustomerRepository : IRepository<CustomerEntity>
    {
        private readonly CustomerDbContext _db;
        public CustomerRepository(CustomerDbContext db) => _db = db;

        public async Task AddAsync(CustomerEntity entity, CancellationToken cancellationToken = default)
        {
            await _db.Customers.AddAsync(entity, cancellationToken);
        }

        public async Task<CustomerEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Customers.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<CustomerEntity>> FindAsync(Expression<Func<CustomerEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _db.Customers.Where(predicate).ToListAsync(cancellationToken);
        }
        public void Remove(CustomerEntity entity) => _db.Customers.Remove(entity);

        public void Update(CustomerEntity entity) => _db.Customers.Update(entity);
    }
}
