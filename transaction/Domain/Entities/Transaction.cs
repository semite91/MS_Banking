using System;

namespace Transaction.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Type { get; private set; }
        public string Status { get; private set; }
        public string? Reference { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Transaction() { }

        public Transaction(Guid accountId, decimal amount, string currency, string type, string? reference = null)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Type = type ?? "Credit";
            Status = "Pending";
            Reference = reference;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkSettled() => Status = "Settled";
        public void MarkFailed() => Status = "Failed";
    }
}
