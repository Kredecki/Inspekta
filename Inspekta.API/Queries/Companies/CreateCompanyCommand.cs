using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Companies;

public sealed record CreateCompanyCommand(
    CompanyDto Company,
    Guid AdminId) : IRequest<CompanyDto>;

public class CreateCompanyCommandHandler(ICompaniesRepository companiesRepository) : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        bool isCompanyAlredyExist = await companiesRepository.IsCompanyAlreadyExist(request.Company, cancellationToken);
        if (!isCompanyAlredyExist)
        {
            await companiesRepository.CreateAsync(request.Company, request.AdminId, cancellationToken);

            return request.Company;
        }

        throw new Exception("E021");
    }
}