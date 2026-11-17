using Application.Abstractions.Interfaces;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Services;
using Application.Options;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Implementations.Services;

internal sealed class RefreshTokenService(
    IRefreshTokenRepository repository,
    IReadOnlyRepository<RefreshToken> readOnlyRepo,
    IUnitOfWork unitOfWork,
    IRefreshTokenGeneratorService refreshTokenGenerator,
    IStringHasher stringHasher,
    TimeProvider timeProvider,
    IOptions<RefreshTokenOptions> rtOptions) : IRefreshTokenService
{

    public async Task<Result<string>> NewRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var newRefreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var utcNow = timeProvider.GetUtcNow();

        var refreshTokenModel = RefreshToken.Create(userId, stringHasher.Hash(newRefreshToken), utcNow, utcNow.AddDays(rtOptions.Value.ExpiryDays));

        await repository.AddAsync(refreshTokenModel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newRefreshToken;
    }

    public async Task<Result<string>> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<string>.Failure(Error.Unauthorized("Auth.RefreshTokenInvalid", "Un authenticated"));

        var existingToken = await repository.GetByTokenHashAsync(stringHasher.Hash(refreshToken), cancellationToken);

        if (existingToken is null || existingToken.IsExpired(timeProvider.GetUtcNow()) || existingToken.IsRevoked())
            return Result<string>.Failure(Error.Unauthorized("Auth.RefreshTokenInvalid", "Un authenticated"));

        var newRefreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var utcNow = timeProvider.GetUtcNow();

        var refreshTokenModel = RefreshToken.Create(existingToken.UserId, stringHasher.Hash(newRefreshToken), utcNow, utcNow.AddDays(rtOptions.Value.ExpiryDays));

        existingToken.Revoke(timeProvider.GetUtcNow(), refreshTokenModel.Id);

        await repository.AddAsync(refreshTokenModel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newRefreshToken;
    }

    public async Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Failure(Error.Unauthorized("Auth.RefreshTokenInvalid", "Un authenticated"));

        var existingToken = await repository.GetByTokenHashAsync(stringHasher.Hash(refreshToken), cancellationToken);

        if (existingToken is null || existingToken.RevokedAt is not null)
            return Result.Failure(Error.Unauthorized("Auth.RefreshTokenInvalid", "Un authenticated"));

        existingToken.Revoke(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Guid> GetUserIdByTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => await readOnlyRepo.FirstOrDefaultAsync(rt => rt.UserId, rt => rt.TokenHash == stringHasher.Hash(refreshToken), cancellationToken);
}
