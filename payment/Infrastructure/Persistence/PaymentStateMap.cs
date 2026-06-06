using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Infrastructure.Sagas;

namespace Payment.Infrastructure.Persistence;

public class PaymentStateMap : IEntityTypeConfiguration<PaymentState>
{
    public void Configure(EntityTypeBuilder<PaymentState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.TransactionId);
        entity.Property(x => x.PaymentId);
        entity.Property(x => x.Amount);
        entity.Property(x => x.Currency).HasMaxLength(16);
        entity.Property(x => x.Status).HasMaxLength(32);
        entity.Property(x => x.Created);
        entity.Property(x => x.UpdatedAt);

        // optional: index on TransactionId for quick lookup
        entity.HasIndex(e => e.TransactionId);
    }
}
