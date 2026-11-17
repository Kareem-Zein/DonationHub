namespace Domain.Interfaces.Entities;

public interface IAuditableEntity
{
    public DateTimeOffset? UpdatedAtUtc { get; }

    public void SetUpdatedAtUtc(DateTimeOffset updatedAtUtc);
}
