using Inspekta.Shared.Enums;
using System.Security.Claims;

namespace Inspekta.API.Abstractions.Services;

public interface ICurrentUserService
{
    Guid GetId(ClaimsPrincipal User);
    EUserRole GetRole(ClaimsPrincipal User);
}
