using System;

namespace Fraud.Domain.Entities
{
    public class FraudRule
    {
        public string Id { get; private set; }
        public string? Description { get; private set; }
        public int Severity { get; private set; }
        public bool Enabled { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private FraudRule() { }

        public FraudRule(string id, string? description, int severity, bool enabled = true)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Description = description;
            Severity = severity;
            Enabled = enabled;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
