using FluentValidation;

namespace Application.Features.Auth.Commands.RegisterUser;

internal sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email not valid");

        RuleFor(r => r.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(3).WithMessage("Full name min length is 3");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password min length is 8")
            .Matches("[A-Z]").WithMessage("Password should contain uppercase")
            .Matches("[a-z]").WithMessage("Password chould conatin lowercase");
    }
}
