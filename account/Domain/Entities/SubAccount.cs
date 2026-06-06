using System;

namespace Account.Domain.Entities
{
    public class SubAccount
    {
        public Guid Id { get; private set; }
        public Guid ParentAccountId { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        private SubAccount() { }

        public SubAccount(Guid parentAccountId, string name)
        {
            Id = Guid.NewGuid();
            ParentAccountId = parentAccountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Balance = 0m;
        }

        public void Credit(decimal amount) {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Balance += amount;
        }

        public void Debit(decimal amount) {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > Balance) throw new InvalidOperationException("Insufficient funds in subaccount");
            Balance -= amount;
        }
    }
}
