using MediatR;
using Payment.Domain.Entities;
using Payment.Infrastructure.Repositories;
using Payment.Infrastructure.UnitOfWork;
using Shared.Infrastructure.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Commands.CreatePayment
{
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, System.Guid>
    {
        private readonly IRepository<PaymentEntity> _repo;
        private readonly IUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public CreatePaymentHandler(IRepository<PaymentEntity> repo, IUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<System.Guid> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = new PaymentEntity(request.Amount, request.Currency, request.Method, request.Provider);
            await _repo.AddAsync(payment, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var txId = request.TransactionId ?? Guid.Empty;
            await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.PaymentProcessed(payment.Id, txId, payment.Amount, payment.Currency, payment.Status), cancellationToken);

            return payment.Id;
        }
    }
}
