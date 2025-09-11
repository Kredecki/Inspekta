using Inspekta.Persistance.Entities;

namespace Inspekta.Persistance.Abstractions.Repositories;

public interface ICompaniesRepository
{
    public Task<Company?> GetCompanyById(Guid id,
        CancellationToken cancellationToken = default);

    public Task<List<Company>> GetCompanies(CancellationToken cancellationToken = default);
}
