using Transaction.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Transaction.Infrastructure.Persistence
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

        public DbSet<Transaction.Domain.Entities.Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction.Domain.Entities.Transaction>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Amount).IsRequired();
                b.Property(x => x.Currency).IsRequired();
                b.Property(x => x.Status).IsRequired();
            });
        }
    }
}
