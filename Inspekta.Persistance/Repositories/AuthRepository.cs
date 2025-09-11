using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance.Repositories;

public class AuthRepository(ApplicationDbContext dbContext) : IAuthRepository
{
	public async Task<bool> IsLoginAlreadyExist(string login, CancellationToken cancellationToken = default)
	{
		bool result = await dbContext.Users.AnyAsync(
			x => x.Login.ToLower().Trim() == login.ToLower().Trim(),
			cancellationToken);

		return !result;
	}

	public async Task<User?> Create(string login, string passwordHash, string salt, EUserRole role, Company company,
        CancellationToken cancellationToken = default)
	{
		User user = new()
		{
			Login = login,
			PassHash = passwordHash,
			Salt = salt,
			Role = role,
			Company = company,
            CreatedBy = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			ModifiedBy = Guid.NewGuid(),
			ModifiedAt = DateTime.UtcNow
		};

		await dbContext.Users.AddAsync(user, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return user;
	}

	public async Task<User?> CheckAuthCredentialsAsync(string login, string password,
		CancellationToken cancellationToken = default)
	{
		User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

		if (user is null)
			return null;

		return user;
	}
}
