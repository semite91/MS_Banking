using Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Persistence
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

        public DbSet<Customer.Domain.Entities.Customer> Customers { get; set; }
        public DbSet<Customer.Domain.Entities.Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Entities.Customer>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.FirstName).IsRequired();
                b.Property(x => x.LastName).IsRequired();
            });

            modelBuilder.Entity<Customer.Domain.Entities.Address>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Line1).IsRequired();
                b.HasOne<Customer.Domain.Entities.Customer>().WithMany("Addresses").HasForeignKey("CustomerId");
            });
        }
    }
}
