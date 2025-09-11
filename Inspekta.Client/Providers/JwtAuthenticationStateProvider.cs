using Blazored.LocalStorage;
using Inspekta.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Inspekta.Client.Providers;

public class JwtAuthenticationStateProvider(ILocalStorageService LocalStorage) : AuthenticationStateProvider
{
	private readonly ILocalStorageService _localStorage = LocalStorage;
	private const string TokenKey = "authToken";

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		string? token = await _localStorage.GetItemAsync<string>(TokenKey);

		if (string.IsNullOrEmpty(token))
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

		JwtSecurityTokenHandler handler = new();
		JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);

		if (jwtToken.ValidTo < DateTime.UtcNow)
		{
			await _localStorage.RemoveItemAsync(TokenKey);
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		ClaimsPrincipal? user = new(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
		return new AuthenticationState(user);
	}

	public async Task Login(UserDto model)
	{
		await _localStorage.SetItemAsync(TokenKey, model.Token);

		List<Claim>? claims = ParseClaimsFromJwt(model.Token).ToList();
		claims.Add(new Claim(ClaimTypes.Name, model.Login));

		ClaimsIdentity? identity = new(claims, TokenKey);
		ClaimsPrincipal? user = new(identity);

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public async Task Logout()
	{
		await _localStorage.RemoveItemAsync(TokenKey);
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
	}

	private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
	{
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(token);

		return jwt.Claims;
	}
}
