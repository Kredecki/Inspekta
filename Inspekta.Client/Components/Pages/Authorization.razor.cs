using Inspekta.Client.Providers;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Inspekta.Client.Components.Pages;

public partial class Authorization
{
	[Inject]
	private JwtAuthenticationStateProvider? AuthProvider { get; set; }
	private static readonly SignInDto Model = new();

    private async Task SignIn()
	{
        HttpResponseMessage? response = await _HttpClient.PostAsJsonAsync<SignInDto>("Api/Authorization/SignIn", Model);

		if (response is null)
			return;

		if (response.IsSuccessStatusCode)
		{
			SignInDto? authResponse = await response.Content.ReadFromJsonAsync<SignInDto>();
			if (authResponse is null) return;

			await AuthProvider!.Login(authResponse);

			Toast.ShowSuccess(T("login_successful"));
			Navigation.NavigateTo("/", forceLoad: true);
		}
		else 
		{
            InspektaError? error = await response.Content.ReadFromJsonAsync<InspektaError>(); 
			
			if(error is not null)
				Toast.ShowError(T("login_failed") + T(error.Detail!));
        }
    }
}
