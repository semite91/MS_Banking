using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using MediatR;
using Fraud.Application.Commands.RaiseFraudAlert;

namespace Fraud.Infrastructure.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessed>
{
    private readonly IMediator _mediator;

    public PaymentProcessedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var msg = context.Message;

        // Simple heuristic: flag failed payments
        if (string.Equals(msg.Status, "Failed", StringComparison.OrdinalIgnoreCase))
        {
            var cmd = new RaiseFraudAlertCommand(msg.TransactionId == Guid.Empty ? null : msg.TransactionId, msg.PaymentId, 0.9m, "PAYMENT_FAILED", "Payment processing failed");
            await _mediator.Send(cmd);
        }
    }
}
