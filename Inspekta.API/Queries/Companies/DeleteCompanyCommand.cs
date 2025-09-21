using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using MediatR;

namespace Inspekta.API.Queries.Companies;

public sealed record DeleteCompanyCommand(Guid Id) : IRequest;

public class DeleteCompanyCommandHandler(ICompaniesRepository companiesRepository) : IRequestHandler<DeleteCompanyCommand>
{
    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        Company? company = await companiesRepository.GetCompanyById(request.Id, cancellationToken) ?? 
            throw new Exception($"E026");

		await companiesRepository.DeleteAsync(company, cancellationToken);
    }
}
