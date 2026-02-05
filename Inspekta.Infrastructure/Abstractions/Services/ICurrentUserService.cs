using Inspekta.Shared.Enums;

namespace Inspekta.Infrastructure.Abstractions.Services;

public interface ICurrentUserService
{
    public Guid GetId();
    public EUserRole GetRole();
}
