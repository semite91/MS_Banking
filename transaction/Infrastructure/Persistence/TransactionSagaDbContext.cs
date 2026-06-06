using Microsoft.EntityFrameworkCore;
using Transaction.Infrastructure.Sagas;

namespace Transaction.Infrastructure.Persistence;

public class TransactionSagaDbContext : DbContext
{
    public TransactionSagaDbContext(DbContextOptions<TransactionSagaDbContext> options) : base(options)
    {
    }

    public DbSet<TransactionState> TransactionStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransactionStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
