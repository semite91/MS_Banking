using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customer.Infrastructure.Sagas;

namespace Customer.Infrastructure.Persistence;

public class CustomerStateMap : IEntityTypeConfiguration<CustomerState>
{
    public void Configure(EntityTypeBuilder<CustomerState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.CustomerId);
        entity.Property(x => x.AccountId);
        entity.Property(x => x.LoanId);
        entity.Property(x => x.Created);
        entity.Property(x => x.UpdatedAt);
    }
}
