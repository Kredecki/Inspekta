namespace Inspekta.API.Abstractions.Services;

public interface IPasswordService
{
	public string GenerateNewSalt();

	public string HashPassword(string password, string salt);
}
