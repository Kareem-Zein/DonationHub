using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Extensions
{
    internal static class ResultExtensions
    {
        public static Result ToResult(this IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
                return Result.Success();

            return Result.Failure(
                identityResult.Errors.Select(MapError).ToArray());
        }
        
        public static Result<T> ToResult<T>(this IdentityResult identityResult, T value)
        {
            if (identityResult.Succeeded)
                return Result<T>.Success(value);

            return Result<T>.Failure(
                identityResult.Errors.Select(MapError).ToArray());
        }

        private static Error MapError(IdentityError error) => error.Code switch
        {
            nameof(IdentityErrorDescriber.DuplicateEmail)
                => Error.Conflict("Identity.DuplicateEmail", error.Description),

            nameof(IdentityErrorDescriber.DuplicateUserName)
                => Error.Conflict("Identity.DuplicateUserName", error.Description),

            nameof(IdentityErrorDescriber.InvalidEmail)
                => Error.Validation("Identity.InvalidEmail", error.Description),

            nameof(IdentityErrorDescriber.PasswordTooShort)
                => Error.Validation("Identity.PasswordTooShort", error.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresDigit)
                => Error.Validation("Identity.PasswordRequiresDigit", error.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresUpper)
                => Error.Validation("Identity.PasswordRequiresUpper", error.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresLower)
                => Error.Validation("Identity.PasswordRequiresLower", error.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)
                => Error.Validation("Identity.PasswordRequiresNonAlphanumeric", error.Description),

            _ => Error.Failure($"Identity.{error.Code}", error.Description)
        };
    }
}
