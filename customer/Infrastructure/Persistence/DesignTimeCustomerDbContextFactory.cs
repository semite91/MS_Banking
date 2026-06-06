using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Customer.Infrastructure.Persistence
{
    public class DesignTimeCustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CustomerDbContext>();
            var conn = Environment.GetEnvironmentVariable("CUSTOMER_CONNECTION") ?? "Host=localhost;Database=Banking_User;Username=postgres;Password=!P1o1s1t1";
            builder.UseNpgsql(conn);
            return new CustomerDbContext(builder.Options);
        }
    }
}
