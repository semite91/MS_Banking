using MassTransit;
using Shared.Infrastructure.Abstractions.Messaging;
using Shared.Infrastructure.Abstractions;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessed>
{
    private readonly IRepository<Transaction.Domain.Entities.Transaction> _repo;
    private readonly IUnitOfWork _uow;

    public PaymentProcessedConsumer(IRepository<Transaction.Domain.Entities.Transaction> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var evt = context.Message;
        if (evt.TransactionId == Guid.Empty) return;

        var tx = await _repo.GetAsync(evt.TransactionId, context.CancellationToken);
        if (tx == null) return;

        if (evt.Status.Equals("Succeeded", StringComparison.OrdinalIgnoreCase) || evt.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            tx.MarkSettled();
        else
            tx.MarkFailed();

        _repo.Update(tx);
        await _uow.SaveChangesAsync(context.CancellationToken);
    }
}
