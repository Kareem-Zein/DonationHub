using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.LogoutCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RefreshCommand;
using Application.Features.Auth.RegisterCommand;
using Domain.Abstractions;

namespace Application.Abstractions.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginCommandResultDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken);
    Task<Result<RefreshCommandResultDto>> RefreshAsync(RefreshCommandDto dto, CancellationToken cancellationToken);
    Task<Result> LogoutAsync(LogoutCommandDto dto, CancellationToken cancellationToken);
    Task<Result<ProfileQueryDto>> ProfileAsync(CancellationToken cancellationToken);
}
