using API.Extensions;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public static class ControllerResultExtensions
{
    public static ActionResult ToActionResult(this Result result, HttpContext context)
        => result.IsSuccess ? new NoContentResult() : BuildFailure(result.Errors, context);

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, HttpContext context)
        => result.IsSuccess
            ? new OkObjectResult(result.Value)
            : BuildFailure(result.Errors, context);

    private static ActionResult BuildFailure(IReadOnlyList<Error> errors, HttpContext context)
    {
        if (errors.Count == 0)
            throw new InvalidOperationException(
                "A failed Result must contain at least one error.");

        var statusCode = errors[0].Type.ToStatusCode();
        var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var title = errors[0].Type switch
        {
            ErrorType.Validation => "Validation Failed",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Server Error"
        };

        var detail = errors.Count == 1
            ? errors[0].Message
            : "One or more errors occurred. See 'errors' for details.";

        var distinctCodes = errors.Select(e => e.Code).Distinct().ToArray();
        var code = distinctCodes.Length == 1
            ? distinctCodes[0]
            : $"{title.Replace(" ", "")}.Multiple";

        var problem = factory.CreateProblemDetails(
            context,
            statusCode: statusCode,
            title: title,
            detail: detail,
            type: $"https://httpstatuses.io/{statusCode}");

        problem.Extensions["code"] = code;
        problem.Extensions["errors"] = errors
            .Select(e => new { code = e.Code, message = e.Message })
            .ToArray();

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}