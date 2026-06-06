using System;

namespace Payment.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Method { get; private set; }
        public string Provider { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Payment() { }

        public Payment(decimal amount, string currency, string method, string provider)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Status = "Initiated";
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAuthorized() => Status = "Authorized";
        public void MarkCaptured() => Status = "Captured";
        public void MarkFailed() => Status = "Failed";
    }
}
