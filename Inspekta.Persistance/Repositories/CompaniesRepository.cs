using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
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

	public async Task<bool> IsCompanyAlreadyExist(CompanyDto companyDto,
		CancellationToken cancellationToken = default)
	{
		Company? company = await dbContext.Companies
			.Where(x => x.NIP == companyDto.NIP)
			.AsNoTracking()
			.FirstOrDefaultAsync(cancellationToken);

		if (company is null)
			return false;

        return true;
	}

	public async Task CreateAsync(CompanyDto companyDto, Guid adminId,
		CancellationToken cancellationToken = default)
	{
		Company company = new()
		{
			Id = Guid.NewGuid(),
			Name = companyDto.Name!,
			NIP = companyDto.NIP,
			Street = companyDto.Street,
			ZipCode = companyDto.ZipCode,
            Town = companyDto.Town,
			Email = companyDto.Email,
			Phone = companyDto.Phone,
            ModifiedBy = adminId,
            ModifiedAt = DateTime.Now,
			CreatedBy = adminId,
            CreatedAt = DateTime.Now
        };

		await dbContext.Companies.AddAsync(company, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
    }

	public async Task UpdateAsync(Company company, Guid adminId,
		CancellationToken cancellationToken = default)
	{
		company.ModifiedAt = DateTime.Now;
		company.ModifiedBy = adminId;

		dbContext.Companies.Update(company);
		await dbContext.SaveChangesAsync(cancellationToken);
    }

	public async Task DeleteAsync(Company company,
		CancellationToken cancellationToken = default)
	{
		dbContext.Companies.Remove(company);
		await dbContext.SaveChangesAsync(cancellationToken);
    }
}
