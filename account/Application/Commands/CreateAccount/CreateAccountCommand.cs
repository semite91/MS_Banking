using MediatR;

namespace Account.Application.Commands.CreateAccount
{
    public record CreateAccountCommand(Guid CustomerId, string AccountType, string Currency, decimal InitialBalance) : IRequest<Guid>;
}
