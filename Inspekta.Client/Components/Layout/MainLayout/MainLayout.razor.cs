using Inspekta.Client.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inspekta.Client.Components.Layout.MainLayout;

/// <summary>
/// Represents the main layout component for the client application. 
/// On initialization, it retrieves the current user’s authentication state
/// and automatically redirects unauthenticated users to the “/auth” page.
/// </summary>
public partial class MainLayout
{
	[Inject]
	private JwtAuthenticationStateProvider? AuthProvider { get; set; }

	protected override async Task OnInitializedAsync()
	{
		AuthenticationState? authState = await AuthStateProvider.GetAuthenticationStateAsync();

		if (authState.User?.Identity is { IsAuthenticated: false } ||
			authState.User?.Identity is null)
		{
			Navigation.NavigateTo("/auth", forceLoad: true);
		}
	}

	private async Task LogoutAsync()
	{
		if (AuthProvider is not null)
			await AuthProvider.Logout();

		Navigation.NavigateTo("/auth", forceLoad: true);
	}
}
