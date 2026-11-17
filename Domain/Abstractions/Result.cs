using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstractions;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success() => new(true, Array.Empty<Error>());
    public static Result Failure(params Error[] errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<Error> errors)
        : base(isSuccess, errors) => Value = value;

    public static Result<T> Success(T value) => new(true, value, Array.Empty<Error>());
    public static new Result<T> Failure(params Error[] errors) => new(false, default, errors);

    public static implicit operator Result<T>(T value) => Success(value);
}
