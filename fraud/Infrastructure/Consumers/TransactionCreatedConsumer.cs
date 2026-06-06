using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using MediatR;
using Fraud.Application.Commands.RaiseFraudAlert;

namespace Fraud.Infrastructure.Consumers;

public class TransactionCreatedConsumer : IConsumer<TransactionCreated>
{
    private readonly IMediator _mediator;

    public TransactionCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<TransactionCreated> context)
    {
        var msg = context.Message;

        // Simple rule: flag transactions over threshold
        var threshold = 10000m;
        if (msg.Amount >= threshold)
        {
            var cmd = new RaiseFraudAlertCommand(msg.TransactionId, null, 1.0m, "HIGH_VALUE_TX", $"Amount {msg.Amount} over threshold");
            await _mediator.Send(cmd);
        }
    }
}
