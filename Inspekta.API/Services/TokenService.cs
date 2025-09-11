using Inspekta.API.Abstractions.Services;
using Inspekta.Persistance.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inspekta.API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
	public string GenerateToken(User user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Sid, user.Id.ToString()),
			new(ClaimTypes.Name, user.Login)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59),
			signingCredentials: creds,
			issuer: config.GetSection("Jwt:Issuer").Value,
			audience: config.GetSection("Jwt:Audience").Value);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);

		return jwt;
	}
}
