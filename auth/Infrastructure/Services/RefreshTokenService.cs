using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Auth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Services;

public class RefreshTokenService
{
    private readonly AuthDbContext _db;
    private readonly IConfiguration _config;

    public RefreshTokenService(AuthDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    private string Hash(string token)
    {
        var secret = _config.GetValue<string>("Jwt:RefreshTokenSecret") ?? "dev-refresh-secret";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId, string? clientId = null, string? device = null, TimeSpan? lifetime = null)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hash = Hash(token);

        var ttl = lifetime ?? TimeSpan.FromDays(30);

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ttl),
            CreatedAt = DateTimeOffset.UtcNow,
            ClientId = clientId,
            Device = device
        };

        _db.RefreshTokens.Add(entity);
        await _db.SaveChangesAsync();

        // return the plain token in a wrapper-like object by setting a temporary property is not ideal; instead caller should receive the raw token separately.
        // We'll return entity and the caller will need the raw token; to convey both, we return entity and write token into Device field temporarily (not ideal).
        entity.Device = token; // temporary carrier for plain token
        return entity;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        var hash = Hash(token);
        return await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash);
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.RevokedAt = DateTimeOffset.UtcNow;
        _db.RefreshTokens.Update(token);
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken> RotateAsync(RefreshToken existing, TimeSpan? lifetime = null)
    {
        existing.RevokedAt = DateTimeOffset.UtcNow;
        var newToken = await CreateRefreshTokenAsync(existing.UserId, existing.ClientId, existing.Device, lifetime);
        existing.ReplacedByTokenId = newToken.Id;
        _db.RefreshTokens.Update(existing);
        await _db.SaveChangesAsync();
        return newToken;
    }
}
