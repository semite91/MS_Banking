using Microsoft.EntityFrameworkCore;
using Customer.Infrastructure.Sagas;

namespace Customer.Infrastructure.Persistence;

public class CustomerSagaDbContext : DbContext
{
    public CustomerSagaDbContext(DbContextOptions<CustomerSagaDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerState> CustomerStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
