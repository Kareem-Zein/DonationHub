using Application.Features.Auth.DTOs;
using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RegisterCommand;
using Domain.Abstractions;

namespace Application.Abstractions.Interfaces.Services;

public interface IIdentityService
{
    Task<Result<LoginIdentityResponseDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken);
    Task<Result<Guid>> RegisterAsync(IdentityRegisterDto dto, CancellationToken cancellationToken);
    Task<Result<UserDataDTO>> GetUserDataByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<ProfileQueryDto>> ProfileAsync(Guid userId, CancellationToken cancellationToken);
}
