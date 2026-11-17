using Domain.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class MinimalApiResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess) return TypedResults.NoContent();
        return BuildFailure(result.Errors);
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess) return TypedResults.Ok(result.Value!);
        return BuildFailure(result.Errors);
    }

    private static IResult BuildFailure(IReadOnlyList<Error> errors)
    {
        var firstError = errors[0];

        // التحقق: يعيد HttpValidationProblemDetails (شكل قياسي مع errors dict)
        if (firstError.Type == ErrorType.Validation)
        {
            var validationErrors = errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return TypedResults.ValidationProblem(validationErrors);
        }

        var statusCode = firstError.Type.ToStatusCode();

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = firstError.Code,
            Detail = firstError.Message,
            Type = $"https://httpstatuses.io/{statusCode}"
        };

        // حقول مخصصة تظهر كأعضاء Top-level في JSON
        problem.Extensions["code"] = firstError.Code;
        problem.Extensions["errors"] = errors
            .Select(e => new { code = e.Code, message = e.Message })
            .ToArray();

        return TypedResults.Problem(problem);
    }
}
