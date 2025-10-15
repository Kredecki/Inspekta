using Inspekta.Client.Providers;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

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
    private ClaimsPrincipal User { get; set; } = new(new ClaimsIdentity());

    protected override async Task OnInitializedAsync()
	{
		if (AuthProvider is null)
			return;

        var authState = await AuthProvider.GetAuthenticationStateAsync();
        this.User = authState.User ?? new ClaimsPrincipal(new ClaimsIdentity());
    }

    private async Task LogoutAsync()
	{
		if (AuthProvider is not null)
			await AuthProvider.Logout();

		Navigation.NavigateTo("/auth", forceLoad: true);
	}
}
