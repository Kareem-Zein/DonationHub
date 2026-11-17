using Domain.Entities.Base;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class PaymentAttempt : AuditableEntity
{
    public Guid DonationId { get; private set; }

    public string StripeCheckoutSessionId { get; private set; } = string.Empty;
    public string? StripePaymentIntentId { get; private set; }
    public string? FailureCode { get; private set; }

    public Money Amount { get; private set; } = null!;

    public PaymentAttemptStatus Status { get; private set; }
}
