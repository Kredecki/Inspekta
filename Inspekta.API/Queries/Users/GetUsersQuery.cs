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
    Guid UserId) : IRequest<(List<UserDto>, int Total)>;

public class GetUsersHandler(IUsersRepository usersRepository, ICompaniesRepository companiesRepository) : IRequestHandler<GetUsersQuery, (List<UserDto>, int Total)>
{
    public async Task<(List<UserDto>, int Total)> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        List<User>? users = [];
        int total = default;

        if (request.UserRole is EUserRole.SuperAdministrator)
        {
            List<User> totalUsers = await usersRepository.GetUsers(cancellationToken) ?? throw new Exception("E012");
            total = totalUsers.Count;

            users = await usersRepository.GetPagedUsers(request.CurrentPage, request.RecordsPerPage, cancellationToken) ?? throw new Exception("E012");
        }

        if (request.UserRole is EUserRole.Administrator)
        {
            Company? company = await companiesRepository.GetCompanyByUserId(request.UserId, cancellationToken) ?? throw new Exception("E013");

            List<User> totalUsers = await usersRepository.GetCompanyUsers(company.Id, cancellationToken) ?? throw new Exception("E014");
            total = totalUsers.Count;

            users = await usersRepository.GetPagedCompanyUsers(company.Id, request.CurrentPage, request.RecordsPerPage, cancellationToken) ?? throw new Exception("E014");
        }

        var items = new List<UserDto>(users.Select(x => new UserDto
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

        return (items, total);
    }
}