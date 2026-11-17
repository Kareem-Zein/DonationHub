using Domain.Abstractions;
using Domain.Entities.Base;

namespace Domain.Entities;

public sealed class Category : SoftDeletableEntity
{
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }

    private Category() { }

    public static Result<Category> Create(
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        DateTimeOffset timeNow)
    {
        var errors = ValidateName(nameAr, nameEn);

        if (timeNow == default)
            errors.Add(Error.Validation("Category.TimeNowRequired", "Creation time must be provided."));

        if (errors.Count > 0)
            return Result<Category>.Failure(errors.ToArray());

        var category = new Category
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            DescriptionAr = descriptionAr?.Trim(),
            DescriptionEn = descriptionEn?.Trim()
        };

        category.SetCreatedAtUtc(timeNow);

        return Result<Category>.Success(category);
    }

    public Result SetName(string nameAr, string nameEn)
    {
        var errors = ValidateName(nameAr, nameEn);

        if (errors.Count > 0)
            return Result.Failure(errors.ToArray());

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();

        return Result.Success();
    }

    public Result SetDescription(string? descriptionAr, string? descriptionEn)
    {
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();

        return Result.Success();
    }

    private static List<Error> ValidateName(string nameAr, string nameEn)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(nameAr))
            errors.Add(Error.Validation("Category.NameArRequired", "NameAr cannot be null or whitespace."));

        if (string.IsNullOrWhiteSpace(nameEn))
            errors.Add(Error.Validation("Category.NameEnRequired", "NameEn cannot be null or whitespace."));

        return errors;
    }

    public Result EnsureCanDelete(bool hasCampaigns)
        => hasCampaigns ? Result.Failure(Error.Validation("Category.DeleteRestrict", "Cannot delete category that has campaigns")) : Result.Success();
}