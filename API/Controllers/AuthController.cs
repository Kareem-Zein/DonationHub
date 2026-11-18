using API.Requests.Auth;
using Application.Abstractions.Interfaces.Services;
using Application.Features.Auth.Commands.RegisterUser;
using Application.Features.Auth.LoginCommand;
using Application.Features.Auth.LogoutCommand;
using Application.Features.Auth.Queries.ProfileQuery;
using Application.Features.Auth.RefreshCommand;
using Application.Features.Auth.RegisterCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(IAuthService authService, ISender sender) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginCommandResultDto>> LoginAsync(LoginCommandDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(dto, cancellationToken);
            return result.ToActionResult(HttpContext);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<Guid>> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new RegisterUserCommand(dto.Email, dto.FullName, dto.Password, dto.PrefferLanguage), cancellationToken);
            return result.IsFailure ? result.ToActionResult(HttpContext) : Created("auth/users", result.Value!);
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
