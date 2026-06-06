using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Account.Infrastructure.Sagas;

namespace Account.Infrastructure.Persistence;

public class AccountStateMap : IEntityTypeConfiguration<AccountState>
{
    public void Configure(EntityTypeBuilder<AccountState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.CustomerId);
        entity.Property(x => x.InitialBalance);
        entity.Property(x => x.Created);
        entity.Property(x => x.UpdatedAt);
    }
}
