using System;
using MassTransit;

namespace Fraud.Infrastructure.Sagas
{
    public class FraudState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid TransactionId { get; set; }
        public Guid? PaymentId { get; set; }
        public string? Reason { get; set; }

        public DateTime? RaisedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
