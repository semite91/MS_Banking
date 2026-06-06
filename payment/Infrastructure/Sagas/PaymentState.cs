using System;
using MassTransit;

namespace Payment.Infrastructure.Sagas
{
    public class PaymentState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid TransactionId { get; set; }
        public Guid? PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
