namespace Application.Abstractions.Interfaces.Services;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}
