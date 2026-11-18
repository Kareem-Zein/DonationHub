using Application.Abstractions.Interfaces.Services;
using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.LogoutCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RefreshCommand;
using Application.Features.Auth.RegisterCommand;
using Domain.Abstractions;

namespace Application.Features.Auth;

internal sealed class AuthService(
    IIdentityService identityService, 
    IRefreshTokenService refreshTokenService, 
    IJwtService jwtService,
    ICurrentUserService currentUserService) : IAuthService
{
    public async Task<Result<LoginCommandResultDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken)
    {
        var loginResult = await identityService.LoginAsync(dto, cancellationToken);

        if (loginResult.IsFailure)
            return Result<LoginCommandResultDto>.Failure(loginResult.Errors.ToArray());

        var result = loginResult.Value!;

        if (!result.IsActive)
            return Result<LoginCommandResultDto>.Failure(Error.Forbidden("Auth.AccountDeactivated", "Account deactivated"));

        var refreshToken = await refreshTokenService.NewRefreshTokenAsync(result.UserId, cancellationToken);

        if (refreshToken.IsFailure)
            return Result<LoginCommandResultDto>.Failure(refreshToken.Errors.ToArray());

        var jwt = jwtService.GenerateAccessToken(new(result.UserId, result.Email, result.UserName, result.Roles.AsReadOnly()));

        return Result<LoginCommandResultDto>.Success(new()
        {
            RefreshToken = refreshToken.Value!,
            AccessToken = jwt.AccessToken,
            AccessTokenExpiresAtUtc = jwt.ExpireAtUtc
        });
    }

    public async Task<Result> LogoutAsync(LogoutCommandDto dto, CancellationToken cancellationToken)
        => await refreshTokenService.RevokeAsync(dto.RefreshToken, cancellationToken);

    public async Task<Result<ProfileQueryDto>> ProfileAsync(CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        if (userId == Guid.Empty)
            return Result<ProfileQueryDto>.Failure(Error.Unauthorized("Auth.UnAuthonticated", "Un authenticated"));

        return await identityService.ProfileAsync(userId, cancellationToken);
    }

    public async Task<Result<RefreshCommandResultDto>> RefreshAsync(RefreshCommandDto dto, CancellationToken cancellationToken)
    {
        var userId = await refreshTokenService.GetUserIdByTokenAsync(dto.RefreshToken, cancellationToken);

        if (userId == Guid.Empty)
            return Result<RefreshCommandResultDto>.Failure(Error.Unauthorized("Auth.RefreshTokenInvalid", "Un authenticated"));

        var user = await identityService.GetUserDataByIdAsync(userId, cancellationToken);

        if (user.IsFailure)
            return Result<RefreshCommandResultDto>.Failure(user.Errors.ToArray());

        if (user.Value!.IsActive == false)
            return Result<RefreshCommandResultDto>.Failure(Error.Forbidden("Auth.AccountDeactivated", "Account deactivated"));

        var refreshToken = await refreshTokenService.RefreshAsync(dto.RefreshToken, cancellationToken);

        if (refreshToken.IsFailure)
            return Result<RefreshCommandResultDto>.Failure(refreshToken.Errors.ToArray());

        var userData = user.Value!;

        var jwt = jwtService.GenerateAccessToken(new(userData.UserId, userData.Email, userData.UserName, userData.Roles));

        return Result<RefreshCommandResultDto>.Success(new(refreshToken.Value!, jwt.AccessToken, jwt.ExpireAtUtc));
    }
}
