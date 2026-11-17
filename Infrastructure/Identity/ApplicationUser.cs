using System.Net.Mail;
using Domain.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; private set; } = null!;
    public Language PreferredLanguage { get; private set; } = Language.EN;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? LastLoginAtUtc { get; private set; }

    private ApplicationUser() { }   // ← EF

    public static Result<ApplicationUser> Create(
        string email,
        string userName,
        string fullName,
        Language preferredLanguage,
        DateTimeOffset timeNow)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add(Error.Validation(
                "Identity.EmailRequired",
                "Email is required."));

        else if (!MailAddress.TryCreate(email.Trim(), out _))
            errors.Add(Error.Validation(
                "Identity.InvalidEmailFormat",
                "Email format is invalid."));

        if (string.IsNullOrWhiteSpace(userName))
            errors.Add(Error.Validation(
                "Identity.UserNameRequired",
                "UserName is required."));

        else if (userName.Trim().Length < 3)
            errors.Add(Error.Validation(
                "Identity.UserNameTooShort",
                "UserName must be at least 3 characters."));

        if (string.IsNullOrWhiteSpace(fullName))
            errors.Add(Error.Validation(
                "Identity.FullNameRequired",
                "FullName is required."));

        else if (fullName.Trim().Length < 3)
            errors.Add(Error.Validation(
                "Identity.FullNameTooShort",
                "FullName must be at least 3 characters."));

        if (timeNow == default)
            errors.Add(Error.Validation(
                "Identity.TimeNowRequired",
                "Creation time must be provided."));

        if (!Enum.IsDefined(preferredLanguage))
            errors.Add(Error.Validation("Identity.PrefferLanguageRequired", "Preffer language is required"));

        if (errors.Count > 0)
            return Result<ApplicationUser>.Failure(errors.ToArray());

        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),                    
            Email = email.Trim(),
            UserName = userName.Trim(),
            FullName = fullName.Trim(),
            PreferredLanguage = preferredLanguage,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAtUtc = timeNow.ToUniversalTime()
        };

        return Result<ApplicationUser>.Success(user);
    }


    public void RecordLogin(DateTimeOffset timeNow)
        => LastLoginAtUtc = timeNow.ToUniversalTime();

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Success();   // idempotent

        IsActive = false;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return Result.Success();

        IsActive = true;
        return Result.Success();
    }

    public void ChangePreferredLanguage(Language language)
        => PreferredLanguage = language;
}