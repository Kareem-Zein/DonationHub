using Application.Abstractions.Interfaces.Services;
using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.LogoutCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RefreshCommand;
using Application.Features.Auth.RegisterCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginCommandResultDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(dto, cancellationToken);
            return result.ToActionResult(HttpContext);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterCommandDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.RegisterAsync(dto, cancellationToken);
            return result.IsFailure ? result.ToActionResult(HttpContext) : Created();
        }
        
        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshCommandResultDto>> RefreshAsync(RefreshCommandDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.RefreshAsync(dto, cancellationToken);
            return result.ToActionResult(HttpContext);
        }
        
        [HttpPost("logout")]
        public async Task<ActionResult> Logout(LogoutCommandDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LogoutAsync(dto, cancellationToken);
            return result.ToActionResult(HttpContext);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ProfileQueryDto>> Profile(CancellationToken cancellationToken)
        {
            var result = await authService.ProfileAsync(cancellationToken);
            return result.ToActionResult(HttpContext);
        }
    }
}
