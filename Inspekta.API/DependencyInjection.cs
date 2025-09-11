using Microsoft.EntityFrameworkCore;
using Inspekta.Persistance;
using System.Reflection;
using Inspekta.API.Abstractions.Services;
using Inspekta.API.Services;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Repositories;

namespace Inspekta.API;
public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
		Assembly? assembly = Assembly.GetExecutingAssembly();
		services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

		services.AddCors(options =>
		{
			options.AddPolicy("AllowedOrigins",
				bldr => bldr.WithOrigins(configuration["AllowedOrigin"]!)
								  .AllowAnyHeader()
								  .AllowAnyMethod());
		});

		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("InspektaDb"));
		});

		services.AddScoped<IPasswordService, PasswordService>();
		services.AddScoped<ITokenService, TokenService>();

		services.AddScoped<IAuthRepository, AuthRepository>();
		services.AddScoped<ICompaniesRepository, CompaniesRepository>();

		return services;
	}
}
