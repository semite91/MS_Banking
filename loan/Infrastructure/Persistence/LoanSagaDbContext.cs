using Microsoft.EntityFrameworkCore;
using Loan.Infrastructure.Sagas;

namespace Loan.Infrastructure.Persistence;

public class LoanSagaDbContext : DbContext
{
    public LoanSagaDbContext(DbContextOptions<LoanSagaDbContext> options) : base(options)
    {
    }

    public DbSet<LoanState> LoanStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LoanStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
