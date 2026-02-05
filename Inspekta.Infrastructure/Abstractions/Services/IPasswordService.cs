namespace Inspekta.Infrastructure.Abstractions.Services;

public interface IPasswordService
{
	public string GenerateNewSalt();

	public string HashPassword(string password, string salt);
}
