using Inspekta.Persistance.Entities;
using Inspekta.Shared.Enums;

namespace Inspekta.Persistance.Abstractions.Repositories;

public interface IAuthRepository
{
	public Task<bool> IsLoginAlreadyExist(string login,
		CancellationToken cancellationToken = default);

	public Task<User?> Create(string login, string passwordHash, string salt, EUserRole role,
        CancellationToken cancellationToken = default);

	public Task<User?> CheckAuthCredentialsAsync(string login, string password,
		CancellationToken cancellationToken = default);
}
