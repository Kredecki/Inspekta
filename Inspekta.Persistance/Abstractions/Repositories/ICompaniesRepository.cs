using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;

namespace Inspekta.Persistance.Abstractions.Repositories;

public interface ICompaniesRepository
{
	public Task<Company?> GetCompanyById(Guid id,
		CancellationToken cancellationToken = default);

	public Task<List<Company>> GetPagedCompanies(int currentPage, int recordsPerPage,
		CancellationToken cancellationToken = default);

	public Task<Company?> GetCompanyByUserId(Guid userId,
		CancellationToken cancellationToken = default);

	public Task<bool> IsCompanyAlreadyExist(CompanyDto companyDto,
		CancellationToken cancellationToken = default);

    public Task CreateAsync(CompanyDto companyDto, Guid adminId,
		CancellationToken cancellationToken = default);

	public Task UpdateAsync(Company company, Guid adminId,
		CancellationToken cancellationToken = default);

	public Task DeleteAsync(Company company,
		CancellationToken cancellationToken = default);
}
