using Inspekta.Persistance.Entities;

namespace Inspekta.Persistance.Abstractions.Repositories;

public interface ICompaniesRepository
{
	public Task<Company?> GetCompanyById(Guid id,
		CancellationToken cancellationToken = default);

	public Task<List<Company>> GetPagedCompanies(int currentPage, int recordsPerPage,
		CancellationToken cancellationToken = default);

	public Task<Company?> GetCompanyByUserId(Guid userId,
		CancellationToken cancellationToken = default);
}
