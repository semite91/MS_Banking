using System;
using MassTransit;

namespace Loan.Infrastructure.Sagas
{
    public class LoanState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid LoanId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
