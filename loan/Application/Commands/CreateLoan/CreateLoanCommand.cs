using MediatR;
using System.Collections.Generic;

namespace Loan.Application.Commands.CreateLoan
{
    public record CreateLoanCommand(Guid CustomerId, decimal Principal, int TermMonths, decimal Rate, Dictionary<string, object>? Payload = null) : IRequest<Guid>;
}
