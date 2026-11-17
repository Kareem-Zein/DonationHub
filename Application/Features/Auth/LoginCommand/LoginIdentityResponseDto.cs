namespace Application.Features.Auth.LoginCommand;

public sealed record LoginIdentityResponseDto(Guid UserId, string Email, string UserName, bool IsActive, IList<string> Roles);
