using MediatR;

namespace Payment.Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(decimal Amount, string Currency, string Method, string Provider, Guid? TransactionId = null) : IRequest<System.Guid>;
}
