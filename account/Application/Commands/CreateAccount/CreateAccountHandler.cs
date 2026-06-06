using MediatR;
using Shared.Infrastructure.Abstractions;
using Account.Domain.Entities;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Application.Commands.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IRepository<AccountEntity> _repo;
        private readonly IUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public CreateAccountHandler(IRepository<AccountEntity> repo, IUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var acc = new AccountEntity(Guid.Empty, request.CustomerId, request.AccountType, request.Currency);
            if (request.InitialBalance > 0)
            {
                acc.Credit(request.InitialBalance);
            }

            await _repo.AddAsync(acc, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var accountNumber = acc.AccountId.ToString();
            await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.AccountCreated(acc.AccountId, acc.CustomerId, accountNumber, acc.Balance, acc.Currency), cancellationToken);

            return acc.AccountId;
        }
    }
}
