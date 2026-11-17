namespace Domain.Interfaces.Entities;

public interface ISoftDeletableEntity
{
    public DateTimeOffset? DeletedAtUtc { get; }
    public DateTimeOffset? RestoredAtUtc { get; }
    public bool IsDeleted { get; }

    public void Delete(DateTimeOffset timeNow);
    public void Restore(DateTimeOffset timeNow);
}
