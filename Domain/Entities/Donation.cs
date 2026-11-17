using Domain.Abstractions;
using Domain.Entities.Base;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Donation : EntityBase
{
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }

    public Money Amount { get; private set; } = null!;
    public DonationStatus Status { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    public bool IsPending => Status == DonationStatus.Pending;
    public bool IsSucceeded => Status == DonationStatus.Succeeded;
    public bool IsFailed => Status == DonationStatus.Failed;

    private Donation() { }  

    public static Result<Donation> Create(
        Guid campaignId,
        Guid donorUserId,
        Money amount,
        DateTimeOffset timeNow)
    {
        var errors = new List<Error>();

        if (campaignId == Guid.Empty)
            errors.Add(Error.Validation(
                "Donation.CampaignRequired",
                "A valid campaign is required."));

        if (donorUserId == Guid.Empty)
            errors.Add(Error.Validation(
                "Donation.DonorRequired",
                "A valid donor is required."));

        if (amount is null || amount.Amount <= 0m)
            errors.Add(Error.Validation(
                "Donation.AmountInvalid",
                "Donation amount must be greater than zero."));

        if (timeNow == default)
            errors.Add(Error.Validation(
                "Donation.TimeNowRequired",
                "Creation time must be provided."));

        if (errors.Count > 0)
            return Result<Donation>.Failure(errors.ToArray());

        var donation = new Donation
        {
            CampaignId = campaignId,
            DonorUserId = donorUserId,
            Amount = amount!,
            Status = DonationStatus.Pending
        };
        donation.SetCreatedAtUtc(timeNow.ToUniversalTime());

        return Result<Donation>.Success(donation);
    }

    public Result MarkCompleted(
        string receiptNumber,
        string? receiptFileName,
        DateTimeOffset timeNow)
    {
        if (IsSucceeded)
            return Result.Success(); 

        if (!IsPending)
            return Result.Failure(Error.Conflict(
                "Donation.InvalidTransition",
                $"Cannot complete a donation in '{Status}' state."));

        if (string.IsNullOrWhiteSpace(receiptNumber))
            return Result.Failure(Error.Validation(
                "Donation.ReceiptNumberRequired",
                "ReceiptNumber is required when completing a donation."));

        if (timeNow == default)
            return Result.Failure(Error.Validation(
                "Donation.TimeNowRequired",
                "Completion time must be provided."));

        Status = DonationStatus.Succeeded;
        ReceiptNumber = receiptNumber.Trim();
        ReceiptFileName = receiptFileName?.Trim();
        CompletedAtUtc = timeNow.ToUniversalTime();

        return Result.Success();
    }


    public Result MarkFailed(string reason, DateTimeOffset timeNow)
    {
        if (IsFailed)
            return Result.Success();  

        if (!IsPending)
            return Result.Failure(Error.Conflict(
                "Donation.InvalidTransition",
                $"Cannot fail a donation in '{Status}' state."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation(
                "Donation.FailureReasonRequired",
                "Failure reason is required."));

        if (timeNow == default)
            return Result.Failure(Error.Validation(
                "Donation.TimeNowRequired",
                "Failure time must be provided."));

        Status = DonationStatus.Failed;
        FailureReason = reason.Trim();

        return Result.Success();
    }

    public Result AttachReceiptFile(string fileName)
    {
        if (!IsSucceeded)
            return Result.Failure(Error.Conflict(
                "Donation.ReceiptRequiresCompletion",
                "Cannot attach a receipt file before the donation is completed."));

        if (string.IsNullOrWhiteSpace(fileName))
            return Result.Failure(Error.Validation(
                "Donation.FileNameRequired",
                "Receipt file name cannot be empty."));

        ReceiptFileName = fileName.Trim();
        return Result.Success();
    }
}