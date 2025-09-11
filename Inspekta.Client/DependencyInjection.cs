using Blazored.LocalStorage;
using Inspekta.Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inspekta.Client;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		string baseUrl = configuration["URL:BaseUrl"] ?? string.Empty;

		services.AddHttpClient("Inspekta", client =>
		{
			client.BaseAddress = new Uri(baseUrl);
		});

		services.AddScoped(sp =>
			sp.GetRequiredService<IHttpClientFactory>().CreateClient("Inspekta")
		);

		services.AddBlazoredLocalStorage();
		services.AddAuthorizationCore();

		services.AddScoped<JwtAuthenticationStateProvider>();
		services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

		return services;
	}
}
