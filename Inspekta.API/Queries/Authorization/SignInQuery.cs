using Inspekta.API.Abstractions.Services;
using Inspekta.API.Exceptions;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Authorization;

public sealed record SignInQuery(SignInDto Dto) : IRequest<SignInDto>;

public class SignInQueryHandler(IAuthRepository authRepository, ITokenService tokenService, IPasswordService passwordService) : IRequestHandler<SignInQuery, SignInDto>
{
	public async Task<SignInDto> Handle(SignInQuery request, CancellationToken cancellationToken)
	{
		if (request.Dto.Login is null)
			throw new InspektaValidationException("login_null");

		if (request.Dto.Password is null)
			throw new InspektaValidationException("password_null");

		User? user =
			await authRepository.CheckAuthCredentialsAsync(request.Dto.Login, cancellationToken) ?? throw new InspektaValidationException("user_doesnt_exist");

		string? password = passwordService.HashPassword(request.Dto.Password, user.Salt);

		if (password != user.PassHash)
			throw new InspektaValidationException("password_is_incorrect");

		return new SignInDto
        {
			Id = user.Id,
			Login = user.Login,
			Token = tokenService.GenerateToken(user),
			Role = user.Role
		};
	}
}