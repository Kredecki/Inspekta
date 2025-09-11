using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance.Repositories;

public class CompaniesRepository(ApplicationDbContext dbContext) : ICompaniesRepository
{
    public async Task<Company?> GetCompanyById(Guid id, 
        CancellationToken cancellationToken = default)
    {
        Company? company = await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (company is null)
            return null;

        return company;
    }

    public async Task<List<Company>> GetCompanies(CancellationToken cancellationToken = default)
    {
        List<Company> companies = await dbContext.Companies.ToListAsync(cancellationToken);

        return companies;
    }
}
