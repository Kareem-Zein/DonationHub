using Application.Abstractions.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;

namespace API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetCurrentUserId()
    {
        var claim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (claim is null)
            return Guid.Empty;

        if (Guid.TryParse(claim.Value, out var userId))
            return userId;

        return Guid.Empty;
    }
}
