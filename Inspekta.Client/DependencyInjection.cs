using Blazored.LocalStorage;
using Inspekta.Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace Inspekta.Client;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		string baseUrl = configuration["URL:BaseUrl"] ?? string.Empty;

		services.AddTransient<JwtAuthorizationMessageHandler>();

		services.AddHttpClient("Inspekta", client =>
		{
			client.BaseAddress = new Uri(baseUrl);
		})
		.AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

		services.AddScoped(sp =>
			sp.GetRequiredService<IHttpClientFactory>().CreateClient("Inspekta")
		);

		services.AddBlazoredLocalStorage();
		services.AddAuthorizationCore(config =>
		{
			foreach (var policy in AppPolicies.Policies)
			{
				config.AddPolicy(policy.Key, policy.Value);
			}
		});

		services.AddScoped<JwtAuthenticationStateProvider>();
		services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

		return services;
	}
}
