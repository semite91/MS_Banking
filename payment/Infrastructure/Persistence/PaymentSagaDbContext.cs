using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Sagas;

namespace Payment.Infrastructure.Persistence;

public class PaymentSagaDbContext : DbContext
{
    public PaymentSagaDbContext(DbContextOptions<PaymentSagaDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentState> PaymentStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
