using Domain.Abstractions;
using Domain.Entities.Base;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Campaign : SoftDeletableEntity
{
    public string TitleAr { get; private set; } = null!;
    public string TitleEn { get; private set; } = null!;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public string? BannerFileName { get; private set; }

    public Money GoalAmount { get; private set; } = null!;
    public Money RaisedAmount { get; private set; } = null!;

    public DateTimeOffset EndDateUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public CampaignStatus Status { get; private set; }


    private Campaign() { }  

    public static Result<Campaign> Create(
        string titleAr,
        string titleEn,
        string? descriptionAr,
        string? descriptionEn,
        Money goalAmount,
        DateTimeOffset endDate,
        Guid categoryId,
        Guid userId,
        DateTimeOffset timeNow)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(titleAr))
            errors.Add(Error.Validation(
                "Campaign.TitleArRequired",
                "TitleAr cannot be null or whitespace."));

        if (string.IsNullOrWhiteSpace(titleEn))
            errors.Add(Error.Validation(
                "Campaign.TitleEnRequired",
                "TitleEn cannot be null or whitespace."));

        if (goalAmount is null)
            errors.Add(Error.Validation("Campaign.GoalAmountRequired", "GoalAmount requiredo."));

        else if (goalAmount.Amount <= 0m)
            errors.Add(Error.Validation(
                "Campaign.GoalAmountInvalid",
                "GoalAmount must be greater than zero."));

        if (endDate <= timeNow)
            errors.Add(Error.Validation(
                "Campaign.EndDateInPast",
                "EndDate must be in the future."));

        if (categoryId == Guid.Empty)
            errors.Add(Error.Validation(
                "Campaign.CategoryRequired",
                "A valid category is required."));
       
        if (userId == Guid.Empty)
            errors.Add(Error.Validation(
                "Campaign.UserRequired",
                "A valid user is required."));

        if (timeNow == default)
            errors.Add(Error.Validation(
                "Campaign.TimeNowRequired",
                "Creation time must be provided."));

        if (errors.Count > 0)
            return Result<Campaign>.Failure(errors.ToArray());

        var campaign = new Campaign
        {
            TitleAr = titleAr.Trim(),
            TitleEn = titleEn.Trim(),
            DescriptionAr = descriptionAr?.Trim(),
            DescriptionEn = descriptionEn?.Trim(),
            GoalAmount = goalAmount,
            RaisedAmount = Money.Zero(goalAmount.Currency),
            EndDateUtc = endDate.ToUniversalTime(),
            CategoryId = categoryId,
            Status = CampaignStatus.Active,
            CreatedByUserId = userId
        };
        campaign.SetCreatedAtUtc(timeNow);

        return Result<Campaign>.Success(campaign);
    }

    public Result UpdateGoalAmount(Money newGoalAmount, DateTimeOffset timeNow)
    {
        if (IsExpired(timeNow))
            return Result.Failure(Error.Conflict(
                "Campaign.Expired",
                "Cannot donate to an expired campaign."));

        if (IsFinalState())
            return Result.Failure(Error.Validation("Compaign.FinalState", "Cannot donate to completed campaign"));

        if (newGoalAmount is null)
            return Result.Failure(Error.Validation("Campaign.InvalidGoalAmount", "Goal amount is required"));

        if (newGoalAmount.Amount <= 0)
            return Result.Failure(Error.Validation("Campaign.InvalidGoalAmount", "Goal amount must be greater than 0"));

        if (!newGoalAmount.HasSameCurrencyAs(RaisedAmount))
            return Result.Failure(MoneyErrors.CurrencyMismatch);

        if (newGoalAmount < RaisedAmount)
            return Result.Failure(Error.Validation("Campaign.InvalidGoalAmount", "Goal amount must be greater than raised amount"));

        GoalAmount = newGoalAmount;

        return Result.Success();
    }

    public Result Donate(Donation donation, DateTimeOffset timeNow)
    {
        if (donation.Status != DonationStatus.Succeeded)
            return Result.Failure(Error.Validation("Campaign.DonationNotSuccessfully", "Donation status must be Succeeded"));

        if (donation.CampaignId != Id)
            return Result.Failure(Error.Validation("Campaign.InvalidDonationCampaign", "Donation not related to this campaign"));

        var amount = donation.Amount;

        if (amount is null || amount.Amount <= 0m)
            return Result.Failure(Error.Validation(
                "Campaign.DonationAmountInvalid",
                "Donation amount must be greater than zero."));

        if (!string.Equals(amount.Currency, GoalAmount.Currency, StringComparison.Ordinal))
            return Result.Failure(Error.Validation(
                "Campaign.CurrencyMismatch",
                $"Donation currency '{amount.Currency}' does not match campaign currency '{GoalAmount.Currency}'."));

        if (IsExpired(timeNow))
            return Result.Failure(Error.Conflict(
                "Campaign.Expired",
                "Cannot donate to an expired campaign."));

        if (IsFinalState())
            return Result.Failure(Error.Validation("Compaign.FinalState", "Cannot donate to completed campaign"));

        if (!RaisedAmount.HasSameCurrencyAs(amount))
            return Result.Failure(MoneyErrors.CurrencyMismatch);

        RaisedAmount = RaisedAmount.Add(amount);

        if (RaisedAmount >= GoalAmount)
            Status = CampaignStatus.Completed;

        return Result.Success();
    }

    private bool IsFinalState()
        => Status == CampaignStatus.Completed;

    private bool IsExpired(DateTimeOffset timeNow)
        => Status == CampaignStatus.Expired || timeNow.ToUniversalTime() >= EndDateUtc;

    public Result UpdateTitle(string titleAr, string titleEn)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(titleAr))
            errors.Add(Error.Validation(
                "Campaign.TitleArRequired",
                "TitleAr cannot be null or whitespace."));

        if (string.IsNullOrWhiteSpace(titleEn))
            errors.Add(Error.Validation(
                "Campaign.TitleEnRequired",
                "TitleEn cannot be null or whitespace."));

        if (errors.Count > 0)
            return Result.Failure(errors.ToArray());

        TitleAr = titleAr.Trim();
        TitleEn = titleEn.Trim();
        return Result.Success();
    }

    public Result UpdateDescription(string? descriptionAr, string? descriptionEn)
    {
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();

        return Result.Success();
    }

    public Result UpdateEndDate(DateTimeOffset newEndDate, DateTimeOffset timeNow)
    {
        if (newEndDate <= timeNow)
            return Result.Failure(Error.Validation(
                "Campaign.EndDateInPast",
                "New end date must be in the future."));

        if (newEndDate <= EndDateUtc)
            return Result.Failure(Error.Validation(
                "Campaign.EndDateMustBeLater",
                "New end date must be later than the current one."));

        EndDateUtc = newEndDate.ToUniversalTime();

        if (Status == CampaignStatus.Expired)
            Status = CampaignStatus.Active;

        return Result.Success();
    }

    //public Result EnsureCanDelete(bool hasDonations)
    //    => hasDonations ? Result.Failure(Error.Validation("Campaign.DeleteRestrict", "Cannot delete campaign that has donations")) : Result.Success();
}