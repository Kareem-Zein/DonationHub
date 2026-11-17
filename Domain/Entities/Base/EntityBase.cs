namespace Domain.Entities.Base;

public abstract class EntityBase
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public DateTimeOffset CreatedAtUtc { get; private set; }

    protected void SetCreatedAtUtc(DateTimeOffset createdAtUtc)
        => CreatedAtUtc = createdAtUtc.ToUniversalTime();
}
