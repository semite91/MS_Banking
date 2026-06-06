using Fraud.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Persistence
{
    public class FraudDbContext : DbContext
    {
        public FraudDbContext(DbContextOptions<FraudDbContext> options) : base(options) { }

        public DbSet<FraudAlert> FraudAlerts { get; set; }
        public DbSet<FraudRule> FraudRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FraudAlert>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Score).IsRequired();
                b.Property(x => x.RuleId).IsRequired();
                b.Property(x => x.Status).IsRequired();
            });

            modelBuilder.Entity<FraudRule>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Description);
            });
        }
    }
}
