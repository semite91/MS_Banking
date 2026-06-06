using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using System;

namespace Payment.Infrastructure.Sagas
{
    public class PaymentStateMachine : MassTransitStateMachine<PaymentState>
    {
        public State Submitted { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }

        public Event<TransactionCreated> TransactionCreatedEvent { get; private set; }
        public Event<PaymentProcessed> PaymentProcessedEvent { get; private set; }

        public PaymentStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => TransactionCreatedEvent, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => PaymentProcessedEvent, x => x.CorrelateById(m => m.Message.TransactionId));

            Initially(
                When(TransactionCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.TransactionId = context.Message.TransactionId;
                        context.Saga.Amount = context.Message.Amount;
                        context.Saga.Currency = context.Message.Currency;
                        context.Saga.Created = DateTime.UtcNow;
                    })
                    .TransitionTo(Submitted)
            );

            During(Submitted,
                When(PaymentProcessedEvent)
                    .Then(context =>
                    {
                        context.Saga.PaymentId = context.Message.PaymentId;
                        context.Saga.Status = context.Message.Status;
                        context.Saga.UpdatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Completed)
            );
        }
    }
}
