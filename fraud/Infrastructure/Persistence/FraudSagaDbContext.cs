using Microsoft.EntityFrameworkCore;
using Fraud.Infrastructure.Sagas;

namespace Fraud.Infrastructure.Persistence;

public class FraudSagaDbContext : DbContext
{
    public FraudSagaDbContext(DbContextOptions<FraudSagaDbContext> options) : base(options)
    {
    }

    public DbSet<FraudState> FraudStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FraudStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
