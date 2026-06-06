namespace Loan.Infrastructure.UnitOfWork
{
    public interface IMongoUnitOfWork
    {
        // Mongo transactions require replica set and explicit session handling.
        Task StartSessionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task AbortAsync(CancellationToken cancellationToken = default);
    }
}
