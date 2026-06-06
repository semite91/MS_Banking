using MongoDB.Driver;

namespace Loan.Infrastructure.UnitOfWork
{
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        private readonly MongoDB.Driver.IMongoClient _client;
        private IClientSessionHandle? _session;

        public MongoUnitOfWork(MongoDB.Driver.IMongoClient client)
        {
            _client = client;
        }

        public Task StartSessionAsync(CancellationToken cancellationToken = default)
        {
            _session = _client.StartSession();
            _session.StartTransaction();
            return Task.CompletedTask;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null) throw new InvalidOperationException("Session not started");
            await _session.CommitTransactionAsync(cancellationToken);
            _session.Dispose();
            _session = null;
        }

        public async Task AbortAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null) return;
            await _session.AbortTransactionAsync(cancellationToken);
            _session.Dispose();
            _session = null;
        }
    }
}
