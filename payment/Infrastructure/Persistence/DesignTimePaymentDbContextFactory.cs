using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Payment.Infrastructure.Persistence
{
    public class DesignTimePaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PaymentDbContext>();
            var conn = Environment.GetEnvironmentVariable("PAYMENT_CONNECTION") ?? "Host=localhost;Database=Banking_Payment;Username=postgres;Password=!P1o1s1t1";
            builder.UseNpgsql(conn);
            return new PaymentDbContext(builder.Options);
        }
    }
}
