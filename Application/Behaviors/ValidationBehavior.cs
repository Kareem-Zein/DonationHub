using Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest: notnull
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errors = failures
            .Select(f => Error.Validation($"Validation.{f.PropertyName}", f.ErrorMessage)).ToArray();

        return CreateFailureResult(errors);
    }

    private TResponse CreateFailureResult(Error[] errors)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(errors);

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(Result.Failure), new[] { typeof(Error[]) })
            ?? throw new InvalidOperationException("Error cast TResponse To Result<>");

        return (TResponse)failureMethod.Invoke(null, new object[] { errors });
    }
}
