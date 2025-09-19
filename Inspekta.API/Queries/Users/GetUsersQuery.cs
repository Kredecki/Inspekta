using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using MediatR;

namespace Inspekta.API.Queries.Users;

public sealed record GetUsersQuery(
    int CurrentPage, 
    int RecordsPerPage,
    EUserRole UserRole,
    Guid UserId) : IRequest<List<UserDto>>;

public class GetUsersHandler(IUsersRepository usersRepository, ICompaniesRepository companiesRepository) : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        List<User>? users = [];

        if (request.UserRole is EUserRole.SuperAdministrator)
            users = await usersRepository.GetPagedUsers(request.CurrentPage, request.RecordsPerPage, cancellationToken) ?? throw new Exception("E012");

        if(request.UserRole is EUserRole.Administrator)
        {
            Company? company = await companiesRepository.GetCompanyByUserId(request.UserId, cancellationToken) ?? throw new Exception("E013");
            users = await usersRepository.GetPagedCompanyUsers(company.Id, request.CurrentPage, request.RecordsPerPage, cancellationToken) ?? throw new Exception("E014");
        }

        return new List<UserDto>(users.Select(x => new UserDto
        {
            Id = x.Id,
            Login = x.Login,
            Role = x.Role,
            Company = x.Company is not null ? new CompanyDto
            {
                Id = x.Company.Id,
                Name = x.Company.Name,
                NIP = x.Company.NIP,
                Street = x.Company.Street,
                ZipCode = x.Company.ZipCode,
                Town = x.Company.Town,
                Email = x.Company.Email,
                Phone = x.Company.Phone
            } : null
        }));
    }
}