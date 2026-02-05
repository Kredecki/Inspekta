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
	private bool ShowValidationMessages = false;

	private void HandleInvalidSubmit()
        => ShowValidationMessages = true;

    private async Task SignIn()
	{
		CancellationToken ct = new();
        HttpResponseMessage? response = await _HttpClient.PostAsJsonAsync<SignInDto>("Api/Authorization/SignIn", Model, ct);

		if (response is null)
			return;

		if (response.IsSuccessStatusCode)
		{
			SignInDto? result = await response.Content.ReadFromJsonAsync<SignInDto>(ct);
			if (result is null) return;

			await AuthProvider!.Login(result);

			Toast.ShowSuccess(T("login_successful"));
			Navigation.NavigateTo("/", forceLoad: true);
		}
		else 
		{
            InspektaError? error = await response.Content.ReadFromJsonAsync<InspektaError>(ct); 
			
			if(error is not null)
				Toast.ShowError(T("login_failed") + T(error.Detail!));
        }
    }
}
