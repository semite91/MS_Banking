using System;

namespace Fraud.Domain.Entities
{
    public class FraudAlert
    {
        public Guid Id { get; private set; }
        public Guid? TransactionId { get; private set; }
        public Guid? PaymentId { get; private set; }
        public decimal Score { get; private set; }
        public string RuleId { get; private set; }
        public string Status { get; private set; }
        public string? Details { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private FraudAlert() { }

        public FraudAlert(Guid? transactionId, Guid? paymentId, decimal score, string ruleId, string? details = null)
        {
            Id = Guid.NewGuid();
            TransactionId = transactionId;
            PaymentId = paymentId;
            Score = score;
            RuleId = ruleId ?? throw new ArgumentNullException(nameof(ruleId));
            Details = details;
            Status = "Open";
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkInvestigated() => Status = "Investigated";
        public void MarkCleared() => Status = "Cleared";
    }
}
