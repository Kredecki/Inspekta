using Inspekta.API.Abstractions.Services;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Authorization;

public sealed record SignUpQuery(
	UserDto Dto,
	Guid AdminId) : IRequest<UserDto>;

public class SignUpQueryHandler(IAuthRepository authRepository, ICompaniesRepository companiesRepository, IPasswordService passwordService)
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

		Company? company;
		if (request.Dto.Company is null)
		{
			company = await companiesRepository.GetCompanyByUserId(request.AdminId, cancellationToken) ?? throw new Exception("E003");

			request.Dto.Company = new CompanyDto()
			{
				Id = company.Id,
				Name = company.Name,
				NIP = company.NIP,
				Street = company.Street,
				ZipCode = company.ZipCode,
				Town = company.Town,
				Email = company.Email,
				Phone = company.Phone
            };
        }
		else
		{
			company = await companiesRepository.GetCompanyById(request.Dto.Company.Id, cancellationToken) ?? throw new Exception("E004");
        }

		if (request.Dto.Password is null)
			throw new Exception("E005");

		string? salt = passwordService.GenerateNewSalt();
		string? hashedPassword = passwordService.HashPassword(request.Dto.Password, salt);
		User? user = await authRepository.Create(request.Dto.Login, hashedPassword, salt, request.Dto.Role, company, cancellationToken) ?? throw new Exception("E006");

		return new UserDto()
		{
			Login = user.Login,
			Role = user.Role,
			Company = user.Company is not null ? new CompanyDto()
			{
				Id = user.Company.Id,
				Name = user.Company.Name,
				NIP = user.Company.NIP,
				Street = user.Company.Street,
				ZipCode = user.Company.ZipCode,
				Town = user.Company.Town,
				Email = user.Company.Email,
				Phone = user.Company.Phone
			} : null
        };
	}
}