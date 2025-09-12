﻿using Inspekta.API.Abstractions.Services;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Authorization;

public sealed record SignInQuery(UserDto Dto) : IRequest<UserDto>;

public class SignInQueryHandler(IAuthRepository authRepository, ITokenService tokenService, IPasswordService passwordService) : IRequestHandler<SignInQuery, UserDto>
{
	public async Task<UserDto> Handle(SignInQuery request, CancellationToken cancellationToken)
	{
		if (request.Dto.Login is null)
			throw new Exception("E007");

		if (request.Dto.Password is null)
			throw new Exception("E008");

		User? user =
			await authRepository.CheckAuthCredentialsAsync(request.Dto.Login, cancellationToken) ?? throw new Exception("E009");

		string? password = passwordService.HashPassword(request.Dto.Password, user.Salt);

		if (password != user.PassHash)
			throw new Exception("E010");

		return new UserDto
		{
			Id = user.Id,
			Login = user.Login,
			Token = tokenService.GenerateToken(user)
		};
	}
}