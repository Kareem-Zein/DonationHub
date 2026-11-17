using Domain.Entities.Base;

namespace Domain.Entities;

public class ProcessedStripeEvent : EntityBase
{
    public string EventId { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public DateTimeOffset ProcessedAt { get; private set; }
}
