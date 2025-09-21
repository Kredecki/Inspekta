using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Companies;

public sealed record UpdateCompanyCommand(
    CompanyDto Company,
    Guid AdminId) : IRequest<CompanyDto?>;

public class UpdateCompanyCommandHandler(ICompaniesRepository companiesRepository) : IRequestHandler<UpdateCompanyCommand, CompanyDto?>
{
    public async Task<CompanyDto?> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? company = await companiesRepository.GetCompanyById(request.Company.Id, cancellationToken);

        if (company is null)
            return null;

        company.Name = request.Company.Name!;
        company.NIP = request.Company.NIP;
        company.Street = request.Company.Street;
        company.ZipCode = request.Company.ZipCode;
        company.Town = request.Company.Town;
        company.Email = request.Company.Email;
        company.Phone = request.Company.Phone;

        await companiesRepository.UpdateAsync(company, request.AdminId, cancellationToken);

        return request.Company;
    }
}