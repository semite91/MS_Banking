using System;

namespace Shared.Infrastructure.Abstractions.Messaging
{
    // Marker interface for integration events
    public interface IIntegrationEvent
    {
        DateTime OccurredAt { get; }
    }

    // Events published by domain services for other services to consume
    public record CustomerCreated(Guid CustomerId, string FirstName, string LastName, string Email) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    public record AccountCreated(Guid AccountId, Guid CustomerId, string AccountNumber, decimal Balance, string Currency) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    public record TransactionCreated(Guid TransactionId, Guid AccountId, decimal Amount, string Currency, DateTime Timestamp, string Type) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    public record PaymentProcessed(Guid PaymentId, Guid TransactionId, decimal Amount, string Currency, string Status) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    public record FraudAlertRaised(Guid AlertId, Guid TransactionId, string Reason, DateTime RaisedAt) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }

    public record LoanCreated(Guid LoanId, Guid CustomerId, decimal Principal, decimal TermMonths, string Status) : IIntegrationEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
