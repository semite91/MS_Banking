using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Transaction.Infrastructure.Persistence
{
    public class DesignTimeTransactionDbContextFactory : IDesignTimeDbContextFactory<TransactionDbContext>
    {
        public TransactionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TransactionDbContext>();
            var conn = Environment.GetEnvironmentVariable("TRANSACTION_CONNECTION") ?? "Host=localhost;Database=Banking_Transaction;Username=postgres;Password=!P1o1s1t1";
            builder.UseNpgsql(conn);
            return new TransactionDbContext(builder.Options);
        }
    }
}
