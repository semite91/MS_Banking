using System;
using System.Collections.Generic;

namespace Loan.Domain.Entities
{
    public class LoanApplication
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal Principal { get; private set; }
        public int TermMonths { get; private set; }
        public decimal Rate { get; private set; }
        public string Status { get; private set; }
        public Dictionary<string, object> ApplicationData { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private LoanApplication() { }

        public LoanApplication(Guid customerId, decimal principal, int termMonths, decimal rate, Dictionary<string, object>? applicationData = null)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Principal = principal;
            TermMonths = termMonths;
            Rate = rate;
            Status = "Submitted";
            ApplicationData = applicationData ?? new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }

        public void Approve() => Status = "Approved";
        public void Reject() => Status = "Rejected";
    }
}
