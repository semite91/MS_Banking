using MassTransit;
using MediatR;
using Shared.Infrastructure.Abstractions.Messaging;
using Payment.Application.Commands.CreatePayment;

namespace Payment.Infrastructure.Consumers;

public class TransactionCreatedConsumer : IConsumer<TransactionCreated>
{
    private readonly IMediator _mediator;

    public TransactionCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<TransactionCreated> context)
    {
        var evt = context.Message;
        // Auto-initiate a payment for the incoming transaction
        var cmd = new CreatePaymentCommand(evt.Amount, evt.Currency, "Auto", "System", evt.TransactionId);
        await _mediator.Send(cmd);
    }
}
