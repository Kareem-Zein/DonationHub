using Domain.Entities.Base;

namespace Domain.Entities;

public sealed class RefreshToken : EntityBase
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTimeOffset timeNow, DateTimeOffset expiresAtUtc)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc
        };

        refreshToken.SetCreatedAtUtc(timeNow);

        return refreshToken;
    }

    public void Revoke(DateTimeOffset timeNow, Guid? replacedByTokenId = null)
    {
        if (RevokedAt.HasValue)
        {
            throw new InvalidOperationException("Refresh token is already revoked.");
        }

        RevokedAt = timeNow;
        ReplacedByTokenId = replacedByTokenId;
    }

    public bool IsExpired(DateTimeOffset timeNow)
        => timeNow >= ExpiresAtUtc;

    public bool IsRevoked()
        => RevokedAt is not null;
}
