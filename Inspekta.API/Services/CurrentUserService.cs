using Inspekta.API.Abstractions.Services;
using Inspekta.Shared.Enums;
using System.Security.Claims;

namespace Inspekta.API.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid GetId(ClaimsPrincipal User)
        => Guid.Parse(User.FindFirstValue(ClaimTypes.Sid)!);

    public EUserRole GetRole(ClaimsPrincipal User)
    {
        return User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Select(v => Enum.TryParse<EUserRole>(v, ignoreCase: true, out var r) ? (EUserRole?)r : null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .First();
    }
}
