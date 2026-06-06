using System;
using MassTransit;

namespace Transaction.Infrastructure.Sagas
{
    public class TransactionState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid TransactionId { get; set; }
        public Guid? PaymentId { get; set; }
        public string? Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
