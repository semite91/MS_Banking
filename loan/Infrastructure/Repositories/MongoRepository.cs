using Loan.Domain.Entities;
using Loan.Infrastructure.Persistence;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Loan.Infrastructure.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(MongoContext ctx, string collectionName)
        {
            _collection = ctx.LoanApplications.Database.GetCollection<T>(collectionName);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            // Assumes entity has Id property
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) throw new InvalidOperationException("Entity has no Id property");
            var id = (Guid)idProp.GetValue(entity)!;
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }
    }
}
