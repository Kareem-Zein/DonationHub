using Application.Abstractions.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.LoginCommand;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Implementations.Services;

internal sealed class JwtService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : IJwtService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;
    public (string AccessToken, DateTimeOffset ExpireAtUtc) GenerateAccessToken(AccessTokenRequirements dto)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, dto.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("UserName", dto.UserName)
        };

        claims.AddRange(dto.Roles.Select(role => new Claim("role", role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Key));

        var expireTime = timeProvider.GetUtcNow().AddMinutes(_jwt.AccessTokenExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expireTime.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);
    }
}
