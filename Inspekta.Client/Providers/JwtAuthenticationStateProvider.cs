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

		var claims = jwtToken.Claims.ToList();
		CreateClaims(claims);

		var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
		return new AuthenticationState(user);
	}

	public async Task Login(UserDto model)
	{
		await _localStorage.SetItemAsync(TokenKey, model.Token);

		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadJwtToken(model.Token);

		var claims = jwtToken.Claims.ToList();

		CreateClaims(claims, model);

		ClaimsIdentity? identity = new(claims, TokenKey);
		ClaimsPrincipal? user = new(identity);

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
	}

	public async Task Logout()
	{
		await _localStorage.RemoveItemAsync(TokenKey);
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
	}

	private static void CreateClaims(List<Claim> claims, UserDto? model = null)
	{
		if (model is null)
			return;

		claims.Add(new Claim(ClaimTypes.Name, model.Login));
		claims.Add(new Claim(ClaimTypes.Role, model.Role.ToString()));
	}

	public async Task<string> GetToken()
		=> await _localStorage.GetItemAsync<string>(TokenKey) ?? string.Empty;
}
