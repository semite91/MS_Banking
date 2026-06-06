using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Account.Infrastructure.Sagas
{
    public class AccountStateMachine : MassTransitStateMachine<AccountState>
    {
        public State Submitted { get; private set; }
        public State Completed { get; private set; }

        public Event<Shared.Infrastructure.Abstractions.Messaging.AccountCreated> AccountCreatedEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.TransactionCreated> TransactionCreatedEvent { get; private set; }

        public AccountStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => AccountCreatedEvent, x =>
            {
                x.CorrelateById(m => m.Message.AccountId);
                x.SelectId(m => m.Message.AccountId);
            });

            Event(() => TransactionCreatedEvent, x => x.CorrelateById(m => m.Message.AccountId));

            Initially(
                When(AccountCreatedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.CustomerId = ctx.Data.CustomerId;
                        ctx.Instance.InitialBalance = ctx.Data.Balance;
                        ctx.Instance.Created = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted)
            );

            During(Submitted,
                When(TransactionCreatedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Completed)
            );
        }
    }
}
