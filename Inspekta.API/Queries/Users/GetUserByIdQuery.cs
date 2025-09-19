using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using MediatR;

namespace Inspekta.API.Queries.Users;

public sealed record GetUserByIdQuery(
    Guid UserId,
    EUserRole UserRole,
    Guid AdminId) : IRequest<UserDto?>;

public class GetUserByIdHandler(IUsersRepository usersRepository, ICompaniesRepository companiesRepository) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        User? user = null;

        if(request.UserRole is EUserRole.SuperAdministrator)
            user = await usersRepository.GetUserById(request.UserId, cancellationToken) ?? throw new Exception("E015");

        if(request.UserRole is EUserRole.Administrator)
        {
            Company? company = await companiesRepository.GetCompanyByUserId(request.AdminId, cancellationToken) ?? throw new Exception("E016");
            user = await usersRepository.GetCompanyUserById(company.Id, request.UserId, cancellationToken) ?? throw new Exception("E017");
        }

        if(user is null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Role = user.Role,
            Company = new CompanyDto
            {
                Id = user.Company.Id,
                Name = user.Company.Name,
                NIP = user.Company.NIP,
                Street = user.Company.Street,
                ZipCode = user.Company.ZipCode,
                Town = user.Company.Town,
                Email = user.Company.Email,
                Phone = user.Company.Phone
            }
        };
    }
}