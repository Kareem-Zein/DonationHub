namespace Application.Abstractions.Interfaces.Services;

public interface IRefreshTokenGeneratorService
{
    string GenerateRefreshToken();
}
