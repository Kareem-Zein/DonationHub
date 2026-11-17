using Application.Abstractions.Interfaces.Services;
using System.Security.Cryptography;

namespace Infrastructure.Implementations.Services;

internal class RefreshTokenGeneratorService() : IRefreshTokenGeneratorService
{
    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(256));
}
