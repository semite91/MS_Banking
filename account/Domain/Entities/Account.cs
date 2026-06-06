using System;
using System.Collections.Generic;

namespace Account.Domain.Entities
{
    public class Account
    {
        public Guid AccountId { get; private set; }
        public Guid CustomerId { get; private set; }
        public string AccountType { get; private set; }
        public string Currency { get; private set; }
        public decimal Balance { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<SubAccount> _subAccounts = new();
        public IReadOnlyCollection<SubAccount> SubAccounts => _subAccounts.AsReadOnly();

        private Account() { }

        public Account(Guid accountId, Guid customerId, string accountType, string currency)
        {
            AccountId = accountId == Guid.Empty ? Guid.NewGuid() : accountId;
            CustomerId = customerId;
            AccountType = accountType ?? throw new ArgumentNullException(nameof(accountType));
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Balance = 0m;
            Status = "Active";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > Balance) throw new InvalidOperationException("Insufficient funds");
            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddSubAccount(SubAccount sub) {
            _subAccounts.Add(sub ?? throw new ArgumentNullException(nameof(sub)));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
