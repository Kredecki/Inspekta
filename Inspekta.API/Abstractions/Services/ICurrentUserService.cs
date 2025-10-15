using Inspekta.Shared.Enums;
using System.Security.Claims;

namespace Inspekta.API.Abstractions.Services;

public interface ICurrentUserService
{
    public Guid GetId();
    public EUserRole GetRole();
}
