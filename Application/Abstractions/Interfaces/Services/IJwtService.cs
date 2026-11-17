using Application.Features.Auth.DTOs;
using Application.Features.Auth.LoginCommand;

namespace Application.Abstractions.Interfaces.Services;

public interface IJwtService
{
    (string AccessToken, DateTimeOffset ExpireAtUtc) GenerateAccessToken(AccessTokenRequirements dto);
}