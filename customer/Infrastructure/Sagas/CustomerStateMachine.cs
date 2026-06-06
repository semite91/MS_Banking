using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Customer.Infrastructure.Sagas
{
    public class CustomerStateMachine : MassTransitStateMachine<CustomerState>
    {
        public State Onboarding { get; private set; }
        public State Completed { get; private set; }

        public Event<CustomerCreated> CustomerCreatedEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.AccountCreated> AccountCreatedEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.LoanCreated> LoanCreatedEvent { get; private set; }

        public CustomerStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => CustomerCreatedEvent, x =>
            {
                x.CorrelateById(m => m.Message.CustomerId);
                x.SelectId(m => m.Message.CustomerId);
            });

            Event(() => AccountCreatedEvent, x => x.CorrelateById(m => m.Message.CustomerId));
            Event(() => LoanCreatedEvent, x => x.CorrelateById(m => m.Message.CustomerId));

            Initially(
                When(CustomerCreatedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.CustomerId = ctx.Data.CustomerId;
                        ctx.Instance.Created = DateTime.UtcNow;
                    })
                    .TransitionTo(Onboarding)
            );

            During(Onboarding,
                When(AccountCreatedEvent)
                    .Then(ctx => { ctx.Instance.AccountId = ctx.Data.AccountId; ctx.Instance.UpdatedAt = DateTime.UtcNow; }),

                When(LoanCreatedEvent)
                    .Then(ctx => { ctx.Instance.LoanId = ctx.Data.LoanId; ctx.Instance.UpdatedAt = DateTime.UtcNow; })
            );

            DuringAny(
                When(AccountCreatedEvent)
                    .If(ctx => ctx.Instance.AccountId != null && ctx.Instance.LoanId != null,
                        binder => binder.TransitionTo(Completed))
            );
        }
    }
}
