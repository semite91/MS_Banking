using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Fraud.Infrastructure.Persistence
{
    public class DesignTimeFraudDbContextFactory : IDesignTimeDbContextFactory<FraudDbContext>
    {
        public FraudDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FraudDbContext>();
            var conn = Environment.GetEnvironmentVariable("FRAUD_CONNECTION") ?? "Host=localhost;Database=Banking_Fraud;Username=postgres;Password=!P1o1s1t1";
            builder.UseNpgsql(conn);
            return new FraudDbContext(builder.Options);
        }
    }
}
