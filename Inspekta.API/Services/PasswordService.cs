using Inspekta.API.Abstractions.Services;
using System.Security.Cryptography;
using System.Text;

namespace Inspekta.API.Services;

public class PasswordService : IPasswordService
{
	public string GenerateNewSalt()
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		const int length = 16;
		byte[] data = new byte[length];

		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(data);
		}

		StringBuilder result = new(length);
		foreach (byte b in data)
		{
			result.Append(chars[b % chars.Length]);
		}

		return result.ToString();
	}

	public string HashPassword(string password, string salt)
	{
		var sb = new StringBuilder();

		var plusSalt = password + salt;
		var result = SHA256.HashData(Encoding.UTF8.GetBytes(plusSalt));

		foreach (var b in result)
		{
			sb.Append(b.ToString("x2"));
		}

		return sb.ToString();
	}
}
