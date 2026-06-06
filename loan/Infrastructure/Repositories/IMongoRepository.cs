using MongoDB.Driver;
using System.Linq.Expressions;

namespace Loan.Infrastructure.Repositories
{
    public interface IMongoRepository<T> where T : class
    {
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
