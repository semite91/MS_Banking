using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Loan.Infrastructure.Sagas;

namespace Loan.Infrastructure.Persistence;

public class LoanStateMap : IEntityTypeConfiguration<LoanState>
{
    public void Configure(EntityTypeBuilder<LoanState> entity)
    {
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.LoanId);
        entity.Property(x => x.CustomerId);
        entity.Property(x => x.Status).HasMaxLength(32);
        entity.Property(x => x.Created);
        entity.Property(x => x.UpdatedAt);
        entity.HasIndex(e => e.LoanId);
    }
}
