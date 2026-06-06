using MediatR;

namespace Transaction.Application.Commands.CreateTransaction
{
    public record CreateTransactionCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount, string Currency) : IRequest<Guid>;
}
