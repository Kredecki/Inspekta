using Blazored.FluentValidation;
using Inspekta.Client.Providers;
using Inspekta.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace Inspekta.Client.Components.Pages;

public partial class Authorization
{
	[Inject]
	private JwtAuthenticationStateProvider? AuthProvider { get; set; }
	private static readonly SignInDto Model = new();

    private async Task SignIn()
	{
        var response = await _HttpClient.PostAsJsonAsync<SignInDto>("Api/Authorization/SignIn", Model);

		if (response is null)
			return;

		if (response.IsSuccessStatusCode)
		{
			var authResponse = await response.Content.ReadFromJsonAsync<SignInDto>();
			if (authResponse is null) return;

			await AuthProvider!.Login(authResponse);
			Navigation.NavigateTo("/", forceLoad: true);
		}
	}
}
