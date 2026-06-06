using MediatR;

namespace Fraud.Application.Commands.RaiseFraudAlert
{
    public record RaiseFraudAlertCommand(Guid? TransactionId, Guid? PaymentId, decimal Score, string RuleId, string? Details) : IRequest<Guid>;
}
