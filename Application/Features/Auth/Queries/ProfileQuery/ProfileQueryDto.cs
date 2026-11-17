using Domain.Enums;

namespace Application.Features.Auth.Queries.ProfileQuery;

public record ProfileQueryDto(Guid UserId, string Email, string FullName, Language PrefferLanguage, IList<string> Roles);
