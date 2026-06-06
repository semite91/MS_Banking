using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Account.Infrastructure.Persistence
{
    public class DesignTimeAccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AccountDbContext>();
            var conn = Environment.GetEnvironmentVariable("ACCOUNT_CONNECTION") ?? "Host=localhost;Database=Banking_User;Username=postgres;Password=!P1o1s1t1";
            builder.UseNpgsql(conn);
            return new AccountDbContext(builder.Options);
        }
    }
}
