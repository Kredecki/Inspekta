using Inspekta.Persistance.Entities;

namespace Inspekta.API.Abstractions.Services;

public interface ITokenService
{
	public string GenerateToken(User user);
}
