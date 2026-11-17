namespace Application.Features.Auth.DTOs;

public record AccessTokenRequirements(Guid UserId, string Email, string UserName, IReadOnlyList<string> Roles);
