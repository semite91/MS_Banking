using MediatR;
using Shared.Infrastructure.Abstractions;
using Transaction.Domain.Entities;
using TransactionEntity = Transaction.Domain.Entities.Transaction;

namespace Transaction.Application.Commands.CreateTransaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, Guid>
    {
        private readonly IRepository<TransactionEntity> _repo;
        private readonly IUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public CreateTransactionHandler(IRepository<TransactionEntity> repo, IUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var tx = new TransactionEntity(request.FromAccountId, request.Amount, request.Currency, "Transfer", request.ToAccountId.ToString());

            await _repo.AddAsync(tx, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.TransactionCreated(tx.Id, tx.AccountId, tx.Amount, tx.Currency, tx.CreatedAt, tx.Type), cancellationToken);

            return tx.Id;
        }
    }
}
