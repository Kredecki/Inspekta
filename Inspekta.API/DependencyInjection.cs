using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Infrastructure.Abstractions.Services;
using Inspekta.Infrastructure.Abstractions.Helpers;
using Inspekta.Persistance.Repositories;
using Inspekta.Infrastructure.Services;
using Inspekta.Infrastructure.Helpers;
using Inspekta.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Inspekta.Persistance;

namespace Inspekta.API;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
	{
        services.AddHttpContextAccessor();

        #region MediatR
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ICommandQuery).Assembly));
        #endregion

        #region Cors
        services.AddCors(options =>
		{
			options.AddPolicy("AllowedOrigins",
				bldr => bldr.WithOrigins(configuration["AllowedOrigin"]!)
								  .AllowAnyHeader()
								  .AllowAnyMethod());
		});
        #endregion

        #region Database
        services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("InspektaDb"));
		});
        #endregion

        #region Exceptions
        services.AddProblemDetails();
		services.AddExceptionHandler<GlobalExceptionHandler>();
        #endregion

        #region Helpers
        services.AddScoped<IFilterHelper, FilterHelper>();
        #endregion

        #region Services
        services.AddSingleton<IPasswordService, PasswordService>();
		services.AddSingleton<ITokenService, TokenService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();
        #endregion

        #region Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
		services.AddScoped<ICompaniesRepository, CompaniesRepository>();
		services.AddScoped<IUsersRepository, UsersRepository>();
        #endregion

        return services;
	}
}
