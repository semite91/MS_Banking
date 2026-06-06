using System;
using MassTransit;

namespace Account.Infrastructure.Sagas
{
    public class AccountState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }
        public decimal? InitialBalance { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
