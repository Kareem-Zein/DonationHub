namespace Application.Features.Auth.RefreshCommand;

public record RefreshCommandResultDto(string RefreshToken, string AccessToken, DateTimeOffset AccessTokenExpiresAtUtc);
