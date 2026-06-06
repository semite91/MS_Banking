using Customer.Domain.Entities;
using Customer.Infrastructure.Repositories;
using Customer.Infrastructure.UnitOfWork;
using Shared.Infrastructure.Abstractions;
using CustomerEntity = Customer.Domain.Entities.Customer;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Customer.Application.Commands.CreateCustomer
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, System.Guid>
    {
        private readonly IRepository<CustomerEntity> _repo;
        private readonly IUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public CreateCustomerHandler(IRepository<CustomerEntity> repo, IUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<System.Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new CustomerEntity(System.Guid.Empty, request.FirstName, request.LastName, request.BirthDate, request.Email, request.Phone);
            await _repo.AddAsync(customer, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.CustomerCreated(customer.Id, customer.FirstName, customer.LastName, customer.Email), cancellationToken);

            return customer.Id;
        }
    }
}
