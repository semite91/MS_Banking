using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Transaction.Infrastructure.Sagas
{
    public class TransactionStateMachine : MassTransitStateMachine<TransactionState>
    {
        public State Submitted { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }

        public Event<CustomerCreated> CustomerCreatedEvent { get; private set; }
        public Event<TransactionCreated> TransactionCreatedEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.PaymentProcessed> PaymentProcessedEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.FraudAlertRaised> FraudAlertRaisedEvent { get; private set; }

        public TransactionStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => TransactionCreatedEvent, x =>
            {
                x.CorrelateById(m => m.Message.TransactionId);
                x.SelectId(m => m.Message.TransactionId);
            });

            Event(() => PaymentProcessedEvent, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => FraudAlertRaisedEvent, x => x.CorrelateById(m => m.Message.TransactionId));

            Initially(
                When(TransactionCreatedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.TransactionId = ctx.Data.TransactionId;
                        ctx.Instance.Created = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted)
            );

            During(Submitted,
                When(PaymentProcessedEvent, ctx =>
                    ctx.Data.Status != null && (ctx.Data.Status.Equals("Succeeded", StringComparison.OrdinalIgnoreCase) || ctx.Data.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)))
                    .Then(ctx =>
                    {
                        ctx.Instance.PaymentId = ctx.Data.PaymentId;
                        ctx.Instance.Status = ctx.Data.Status;
                        ctx.Instance.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Completed),

                When(PaymentProcessedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.Status = ctx.Data.Status;
                        ctx.Instance.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Failed),

                When(FraudAlertRaisedEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.Status = "Fraud";
                        ctx.Instance.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Failed)
            );
        }
    }
}
