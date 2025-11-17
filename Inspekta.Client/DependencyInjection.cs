using Blazored.LocalStorage;
using Blazored.Toast;
using FluentValidation;
using Inspekta.Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;

namespace Inspekta.Client;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
        #region Http
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
        #endregion

        #region Validator
        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
        #endregion

        #region Toasts
        services.AddBlazoredToast();
        #endregion

        #region Local Storage
        services.AddBlazoredLocalStorage();
        #endregion

        #region Authorization & Authentication
        services.AddAuthorizationCore(config =>
        {
            foreach (var policy in AppPolicies.Policies)
            {
                config.AddPolicy(policy.Key, policy.Value);
            }
        });

        services.AddScoped<JwtAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<JwtAuthenticationStateProvider>());
        #endregion

        #region Localization
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pl-PL");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pl-PL");
        #endregion

        return services;
	}
}
