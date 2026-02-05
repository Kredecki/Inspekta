using Inspekta.Infrastructure.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Inspekta.Shared.Enums;

namespace Inspekta.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid GetId()
    {
        var user = accessor.HttpContext?.User;
        var sid = user?.FindFirstValue(ClaimTypes.Sid);

        return sid is not null && Guid.TryParse(sid, out var id)
            ? id
            : throw new InvalidOperationException("Brak SID w kontekście.");
    }

    public EUserRole GetRole()
    {
        var user = accessor.HttpContext?.User;
        var role = user?.FindAll(ClaimTypes.Role)
                       .Select(c => c.Value)
                       .FirstOrDefault();

        return Enum.TryParse<EUserRole>(role, true, out var r)
            ? r
            : throw new InvalidOperationException("Brak roli w kontekście.");
    }
}
