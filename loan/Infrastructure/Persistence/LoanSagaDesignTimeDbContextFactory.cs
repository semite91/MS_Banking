using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Loan.Infrastructure.Persistence
{
    public class LoanSagaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<LoanSagaDbContext>
    {
        public LoanSagaDbContext CreateDbContext(string[] args)
        {
            var conn = Environment.GetEnvironmentVariable("LOAN_CONNECTION")
                       ?? "Host=localhost;Database=Banking_Loan;Username=postgres;Password=!P1o1s1t1";

            var builder = new DbContextOptionsBuilder<LoanSagaDbContext>();
            builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(LoanSagaDbContext).Assembly.GetName().Name));

            return new LoanSagaDbContext(builder.Options);
        }
    }
}
