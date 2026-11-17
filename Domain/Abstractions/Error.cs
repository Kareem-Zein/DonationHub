using Domain.Enums;

namespace Domain.Abstractions;

public sealed record Error(string Code, ErrorType Type, string Message)
{
    public static readonly Error None = new Error(string.Empty, ErrorType.Failure, string.Empty);
    public static implicit operator Result(Error error) => Result.Failure(error);

    public static Error Validation(string code, string message) => new(code, ErrorType.Validation, message);
    public static Error NotFound(string code, string message) => new(code, ErrorType.NotFound, message);
    public static Error Conflict(string code, string message) => new(code, ErrorType.Conflict, message);
    public static Error Unauthorized(string code, string message) => new(code, ErrorType.Unauthorized, message);
    public static Error Forbidden(string code, string message) => new(code, ErrorType.Forbidden, message);
    public static Error Failure(string code, string message) => new(code, ErrorType.Failure, message);
}
