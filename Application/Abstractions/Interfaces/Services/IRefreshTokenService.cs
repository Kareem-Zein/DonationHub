using Domain.Abstractions;

namespace Application.Abstractions.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<Result<string>> NewRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<string>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Guid> GetUserIdByTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
