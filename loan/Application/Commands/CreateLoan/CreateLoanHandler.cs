using MediatR;
using Loan.Domain.Entities;
using Loan.Infrastructure.Repositories;
using Loan.Infrastructure.UnitOfWork;

namespace Loan.Application.Commands.CreateLoan
{
    public class CreateLoanHandler : IRequestHandler<CreateLoanCommand, Guid>
    {
        private readonly IMongoRepository<LoanApplication> _repo;
        private readonly IMongoUnitOfWork _uow;
        private readonly MassTransit.IPublishEndpoint _publish;

        public CreateLoanHandler(IMongoRepository<LoanApplication> repo, IMongoUnitOfWork uow, MassTransit.IPublishEndpoint publish)
        {
            _repo = repo;
            _uow = uow;
            _publish = publish;
        }

        public async Task<Guid> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
        {
            var app = new LoanApplication(request.CustomerId, request.Principal, request.TermMonths, request.Rate, request.Payload);

            await _uow.StartSessionAsync(cancellationToken);
            try
            {
                await _repo.AddAsync(app, cancellationToken);
                await _uow.CommitAsync(cancellationToken);

                await _publish.Publish(new Shared.Infrastructure.Abstractions.Messaging.LoanCreated(app.Id, app.CustomerId, app.Principal, app.TermMonths, app.Status), cancellationToken);

                return app.Id;
            }
            catch
            {
                await _uow.AbortAsync(cancellationToken);
                throw;
            }
        }
    }
}
