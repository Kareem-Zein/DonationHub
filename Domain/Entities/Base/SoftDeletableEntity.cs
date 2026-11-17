using Domain.Interfaces.Entities;

namespace Domain.Entities.Base;

public class SoftDeletableEntity : AuditableEntity, ISoftDeletableEntity
{
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public DateTimeOffset? RestoredAtUtc { get; private set; }

    public bool IsDeleted { get; private set; }

    public void Delete(DateTimeOffset timeNow)
    {
        DeletedAtUtc = timeNow.ToUniversalTime();
        RestoredAtUtc = null;
        IsDeleted = true;
    }


    public void Restore(DateTimeOffset timeNow)
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAtUtc = null;
        RestoredAtUtc = timeNow.ToUniversalTime();
    }
}
