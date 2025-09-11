using MediatR;
using Inspekta.Shared.DTOs;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.API.Abstractions.Services;

namespace Inspekta.API.Queries.Authorization;

public sealed record SignInQuery(UserDto Dto) : IRequest<UserDto>;

public class SignInQueryHandler(IAuthRepository authRepository, ITokenService tokenService) : IRequestHandler<SignInQuery, UserDto>
{
	public async Task<UserDto> Handle(SignInQuery request, CancellationToken cancellationToken)
	{
		if (request.Dto.Login is null)
			throw new Exception("E007");

		if (request.Dto.Password is null)
			throw new Exception("E008");

        User? user =
			await authRepository.CheckAuthCredentialsAsync(request.Dto.Login, request.Dto.Password, cancellationToken) ?? throw new Exception("E009");

		return new UserDto
		{
			Id = user.Id,
			Login = user.Login,
			Password = user.PassHash,
			Token = tokenService.GenerateToken(user)
		};
	}
}