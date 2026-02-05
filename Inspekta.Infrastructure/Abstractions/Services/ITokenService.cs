using Inspekta.Persistance.Entities;

namespace Inspekta.Infrastructure.Abstractions.Services;

public interface ITokenService
{
	public string GenerateToken(User user);
}
