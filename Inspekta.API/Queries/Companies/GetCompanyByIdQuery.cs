using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Companies;

public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDto?>;

public class GetCompanyByIdQueryHandler(ICompaniesRepository companiesRepository) : IRequestHandler<GetCompanyByIdQuery, CompanyDto?>
{
    public async Task<CompanyDto?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        Company? company = await companiesRepository.GetCompanyById(request.Id, cancellationToken) ?? throw new Exception("E020");

        if (company is null)
            return null;

        return new CompanyDto
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
}