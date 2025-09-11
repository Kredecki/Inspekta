using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Companies;

public sealed record GetCompaniesQuery : IRequest<List<CompanyDto>>;

public class GetCompaniesHandler(ICompaniesRepository companiesRepository) : IRequestHandler<GetCompaniesQuery, List<CompanyDto>>
{
    public async Task<List<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        List<Company>? companies = await companiesRepository.GetCompanies(cancellationToken) ?? throw new Exception("E010");

        return new List<CompanyDto>(companies.Select(x => new CompanyDto
        {
            Id = x.Id,
            Name = x.Name,
            NIP = x.NIP,
            Street = x.Street,
            ZipCode = x.ZipCode,
            Town = x.Town,
            Email = x.Email,
            Phone = x.Phone
        }));
    }
}
