using Domain.Entities.Base;

namespace Domain.Entities;

public class IdempotencyRecord : EntityBase
{
    public string Key { get; private set; } = null!;
    public string RequestHash { get; private set; } = null!;
    public int ResponseStatusCode { get; private set; }
    public string ResponseBody { get; private set; } = null!;

    public Guid UserId { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }
}
