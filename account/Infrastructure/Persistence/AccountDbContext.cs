using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<Account.Domain.Entities.Account> Accounts { get; set; }
        public DbSet<Account.Domain.Entities.SubAccount> SubAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account.Domain.Entities.Account>(b =>
            {
                b.HasKey(x => x.AccountId);
                b.Property(x => x.AccountType).IsRequired();
                b.Property(x => x.Currency).IsRequired();
            });

            modelBuilder.Entity<Account.Domain.Entities.SubAccount>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne<Account.Domain.Entities.Account>().WithMany("SubAccounts").HasForeignKey("ParentAccountId");
            });
        }
    }
}
