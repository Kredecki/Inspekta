using Inspekta.Infrastructure.Exceptions;
using Inspekta.Infrastructure.Abstractions.Helpers;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using MediatR;

namespace Inspekta.Infrastructure.Queries.Users;

public sealed record GetUsersQuery(
    int CurrentPage, 
    int RecordsPerPage,
    string? SearchTerm,
    string? SortColumn,
    bool SortDescending,
    EUserRole UserRole,
    Guid UserId) : IRequest<(List<UserDto>, int Total)>;

public class GetUsersHandler(
    IUsersRepository usersRepository, 
    ICompaniesRepository companiesRepository,
    IFilterHelper filterHelper) : IRequestHandler<GetUsersQuery, (List<UserDto>, int Total)>
{
    public async Task<(List<UserDto>, int Total)> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User>? users = await usersRepository.GetUsers(
                    cancellationToken) ?? throw new InspektaNullException("no_users_found");
        int total = default;

        switch (request.UserRole)
        {
            case EUserRole.Administrator:
                Company? company = await companiesRepository.GetCompanyByUserId(
                    request.UserId, 
                    cancellationToken) ?? throw new InspektaNullException("company_doesnt_exist");

                users = users.Where(users => users.CompanyId == company.Id);
                break;
            default:
                break;
        }

        users = await filterHelper.ApplySearchFilter(
            users,
            request.SearchTerm!,
            x => x.Login);

        users = await filterHelper.ApplySortFilter(
            users,
            request.SortColumn!,
            request.SortDescending);

        total = users.Count();

        users = users
            .Skip(request.CurrentPage * request.RecordsPerPage)
            .Take(request.RecordsPerPage);

        List<UserDto> items = new(users.Select(x => new UserDto
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