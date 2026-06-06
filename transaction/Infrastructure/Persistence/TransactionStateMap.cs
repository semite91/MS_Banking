using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transaction.Infrastructure.Sagas;

namespace Transaction.Infrastructure.Persistence;

public class TransactionStateMap : IEntityTypeConfiguration<TransactionState>
{
    public void Configure(EntityTypeBuilder<TransactionState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.TransactionId);
        entity.Property(x => x.PaymentId);
        entity.Property(x => x.Status).HasMaxLength(32);
        entity.Property(x => x.Created);
        entity.Property(x => x.UpdatedAt);

        entity.HasIndex(e => e.TransactionId);
    }
}
