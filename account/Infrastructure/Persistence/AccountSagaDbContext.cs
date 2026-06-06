using Microsoft.EntityFrameworkCore;
using Account.Infrastructure.Sagas;

namespace Account.Infrastructure.Persistence;

public class AccountSagaDbContext : DbContext
{
    public AccountSagaDbContext(DbContextOptions<AccountSagaDbContext> options) : base(options)
    {
    }

    public DbSet<AccountState> AccountStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountStateMap());
        base.OnModelCreating(modelBuilder);
    }
}
