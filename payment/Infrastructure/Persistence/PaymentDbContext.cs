using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<Payment.Domain.Entities.Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment.Domain.Entities.Payment>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Amount).IsRequired();
                b.Property(x => x.Currency).IsRequired();
                b.Property(x => x.Method).IsRequired();
                b.Property(x => x.Provider).IsRequired();
                b.Property(x => x.Status).IsRequired();
            });
        }
    }
}
