using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Loan.Infrastructure.Sagas
{
    public class LoanStateMachine : MassTransitStateMachine<LoanState>
    {
        public State Requested { get; private set; }
        public State ReadyForFunding { get; private set; }
        public State Funded { get; private set; }

        public Event<Shared.Infrastructure.Abstractions.Messaging.LoanCreated> LoanCreatedEvent { get; private set; }
        public Event<CustomerCreated> CustomerCreatedEvent { get; private set; }

        public LoanStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => LoanCreatedEvent, x =>
            {
                x.CorrelateById(m => m.Message.LoanId);
                x.SelectId(m => m.Message.LoanId);
            });

            Event(() => CustomerCreatedEvent, x => x.CorrelateById(m => m.Message.CustomerId));

            Initially(
                When(LoanCreatedEvent)
                    .Then(ctx => { ctx.Instance.LoanId = ctx.Data.LoanId; ctx.Instance.Created = DateTime.UtcNow; })
                    .TransitionTo(Requested)
            );

            During(Requested,
                When(CustomerCreatedEvent)
                    .Then(ctx => { ctx.Instance.CustomerId = ctx.Data.CustomerId; ctx.Instance.UpdatedAt = DateTime.UtcNow; })
                    .TransitionTo(ReadyForFunding)
            );
        }
    }
}
