using Domain.Interfaces.Entities;

namespace Domain.Entities.Base;

public class AuditableEntity : EntityBase, IAuditableEntity
{
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public void SetUpdatedAtUtc(DateTimeOffset updatedAtUtc)
        => UpdatedAtUtc = updatedAtUtc.ToUniversalTime();
}
