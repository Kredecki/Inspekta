using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance.Repositories;

public class CompaniesRepository(ApplicationDbContext dbContext) : ICompaniesRepository
{
	public async Task<Company?> GetCompanyById(Guid id,
		CancellationToken cancellationToken = default)
	{
		Company? company = await dbContext.Companies
			.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

		if (company is null)
			return null;

		return company;
	}

	public async Task<List<Company>> GetPagedCompanies(int currentPage, int recordsPerPage,
		CancellationToken cancellationToken = default)
	{
		List<Company> companies = await dbContext.Companies
            .Skip(currentPage * recordsPerPage)
            .Take(recordsPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

		return companies;
	}

	public async Task<Company?> GetCompanyByUserId(Guid userId, 
		CancellationToken cancellationToken = default)
	{
		Company? company = await dbContext.Users
			.Where(u => u.Id == userId)
			.Include(u => u.Company)
			.Select(u => u.Company)
			.AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

		return company;
    }
}
