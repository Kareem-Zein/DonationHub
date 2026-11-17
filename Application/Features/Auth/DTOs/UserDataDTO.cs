namespace Application.Features.Auth.DTOs;

public record UserDataDTO(Guid UserId, string Email, string UserName, bool IsActive, IReadOnlyList<string> Roles);
