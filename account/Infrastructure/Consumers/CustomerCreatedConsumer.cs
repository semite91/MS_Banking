using MassTransit;
using MediatR;
using Shared.Infrastructure.Abstractions.Messaging;
using Account.Application.Commands.CreateAccount;

namespace Account.Infrastructure.Consumers;

public class CustomerCreatedConsumer : IConsumer<CustomerCreated>
{
    private readonly IMediator _mediator;

    public CustomerCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CustomerCreated> context)
    {
        var evt = context.Message;
        // Auto-create a default checking account for new customers
        var cmd = new CreateAccountCommand(evt.CustomerId, "Checking", "USD", 0m);
        await _mediator.Send(cmd);
    }
}
