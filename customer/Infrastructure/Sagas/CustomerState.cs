using System;
using MassTransit;

namespace Customer.Infrastructure.Sagas
{
    public class CustomerState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? LoanId { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
