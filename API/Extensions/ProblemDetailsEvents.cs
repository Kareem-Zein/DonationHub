using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;
public static class ProblemDetailsEvents
{
    public static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        if (context.Response.HasStarted) return;

        var factory = context.HttpContext.RequestServices
            .GetRequiredService<IProblemDetailsService>();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await factory.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context.HttpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = context.ErrorDescription ?? "Authentication is required.",
                Type = "https://httpstatuses.io/401"
            }
        });
    }

    public static async Task OnForbidden(ForbiddenContext context)
    {
        if (context.Response.HasStarted) return;

        var factory = context.HttpContext.RequestServices
            .GetRequiredService<IProblemDetailsService>();

        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        await factory.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context.HttpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = "You do not have permission to access this resource.",
                Type = "https://httpstatuses.io/403"
            }
        });
    }
}
