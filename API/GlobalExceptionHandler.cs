
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        if (exception is OperationCanceledException &&
            httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "Request was cancelled by the client. Path: {Path}",
                httpContext.Request.Path);

            return true;
        }

        if (httpContext.Response.HasStarted)
        {
            logger.LogError(
                exception,
                "Unhandled exception after response started. Path: {Path}",
                httpContext.Request.Path);

            return false;
        }

        logger.LogError(
            exception,
            "Unhandled exception on {Path}",
            httpContext.Request.Path);

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server.InternalError",
            Detail = "An unexpected error occurred. Please try again later.",
            Type = "https://httpstatuses.io/500"
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
    }
}
