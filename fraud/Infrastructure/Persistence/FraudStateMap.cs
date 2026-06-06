using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fraud.Infrastructure.Sagas;

namespace Fraud.Infrastructure.Persistence;

public class FraudStateMap : IEntityTypeConfiguration<FraudState>
{
    public void Configure(EntityTypeBuilder<FraudState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.TransactionId);
        entity.Property(x => x.PaymentId);
        entity.Property(x => x.Reason).HasMaxLength(256);
        entity.Property(x => x.RaisedAt);
        entity.Property(x => x.ResolvedAt);
    }
}
