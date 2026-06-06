using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Fraud.Infrastructure.Sagas
{
    public class FraudStateMachine : MassTransitStateMachine<FraudState>
    {
        public State Investigating { get; private set; }
        public State Resolved { get; private set; }

        public Event<Shared.Infrastructure.Abstractions.Messaging.FraudAlertRaised> FraudAlertEvent { get; private set; }
        public Event<Shared.Infrastructure.Abstractions.Messaging.PaymentProcessed> PaymentProcessedEvent { get; private set; }

        public FraudStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => FraudAlertEvent, x =>
            {
                x.CorrelateById(m => m.Message.AlertId);
                x.SelectId(m => m.Message.AlertId);
            });

            Event(() => PaymentProcessedEvent, x => x.CorrelateById(m => m.Message.TransactionId));

            Initially(
                When(FraudAlertEvent)
                    .Then(ctx =>
                    {
                        ctx.Instance.TransactionId = ctx.Data.TransactionId;
                        ctx.Instance.Reason = ctx.Data.Reason;
                        ctx.Instance.RaisedAt = ctx.Data.RaisedAt;
                    })
                    .TransitionTo(Investigating)
            );

            During(Investigating,
                When(PaymentProcessedEvent)
                    .Then(ctx => { ctx.Instance.ResolvedAt = DateTime.UtcNow; })
                    .TransitionTo(Resolved)
            );
        }
    }
}
