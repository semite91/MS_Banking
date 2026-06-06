using Fraud.Domain.Entities;
using Fraud.Infrastructure.Repositories;
using Fraud.Infrastructure.UnitOfWork;
using MediatR;
using Shared.Infrastructure.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Fraud.Application.Commands.RaiseFraudAlert
{
    public class RaiseFraudAlertHandler : IRequestHandler<RaiseFraudAlertCommand, System.Guid>
    {
        private readonly IRepository<FraudAlert> _repo;
        private readonly IUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public RaiseFraudAlertHandler(IRepository<FraudAlert> repo, IUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<System.Guid> Handle(RaiseFraudAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = new FraudAlert(request.TransactionId, request.PaymentId, request.Score, request.RuleId, request.Details);
            await _repo.AddAsync(alert, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.FraudAlertRaised(alert.Id, alert.TransactionId ?? Guid.Empty, request.RuleId, alert.CreatedAt), cancellationToken);

            return alert.Id;
        }
    }
}
