using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence;

public class AuthDbContext : IdentityDbContext<Microsoft.AspNetCore.Identity.IdentityUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<RefreshToken>().HasKey(x => x.Id);
        builder.Entity<RefreshToken>().HasIndex(x => x.TokenHash);
        builder.Entity<RefreshToken>().Property(x => x.CreatedAt).HasDefaultValueSql("now()");
    }
}
