namespace Bank.Shared.Contracts.Events
{
    public record CustomerCreated(Guid CustomerId, string FirstName, string LastName, string Email);
    public record AccountDebited(Guid AccountId, decimal Amount, string Currency);
    public record PaymentInitiated(Guid PaymentId, decimal Amount, string Currency, string Method);
}
