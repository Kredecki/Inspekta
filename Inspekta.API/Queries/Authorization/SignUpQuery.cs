using MediatR;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Inspekta.API.Abstractions.Services;

namespace Inspekta.API.Queries.Authorization;

public sealed record SignUpQuery(UserDto Dto) : IRequest<UserDto>;

public class SignUpQueryHandler(IAuthRepository authRepository, IPasswordService passwordService)
	: IRequestHandler<SignUpQuery, UserDto?>
{
	public async Task<UserDto?> Handle(SignUpQuery request, CancellationToken cancellationToken)
	{
		if (request.Dto.Login is null)
            throw new Exception("E001");

        bool isValidationGood =
			await authRepository.IsLoginAlreadyExist(request.Dto.Login, cancellationToken);

		if (!isValidationGood)
			throw new Exception("E002");

		string? salt = passwordService.GenerateNewSalt();
		string? hashedPassword = passwordService.HashPassword(request.Dto.Password, salt);
		User? user = await authRepository.Create(request.Dto.Login, hashedPassword, salt, request.Dto.Role, cancellationToken);

		if (user is null)
			return null;

		return new UserDto()
		{
			Login = user.Login
		};
	}
}