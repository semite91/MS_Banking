using MassTransit;
using MediatR;
using Shared.Infrastructure.Abstractions.Messaging;
using Loan.Application.Commands.CreateLoan;

namespace Loan.Infrastructure.Consumers;

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
        // Optionally create a starter loan application or onboarding payload; using conservative defaults
        var cmd = new CreateLoanCommand(evt.CustomerId, 0m, 0, 0m, new Dictionary<string, object> { { "Onboarded", true } });
        await _mediator.Send(cmd);
    }
}
