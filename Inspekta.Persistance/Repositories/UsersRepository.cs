using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance.Repositories;

public class UsersRepository(ApplicationDbContext dbContext) : IUsersRepository
{
    public async Task<List<User>> GetPagedUsers(int currentPage, int recordsPerPage, 
        CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Skip(currentPage * recordsPerPage)
            .Take(recordsPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<User>> GetPagedCompanyUsers(Guid companyId, int currentPage, int recordsPerPage, 
        CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Where(u => u.CompanyId == companyId)
            .Skip(currentPage * recordsPerPage)
            .Take(recordsPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<User?> GetUserById(Guid id,
        CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<User?> GetCompanyUserById(Guid id, Guid companyId,
        CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Where(x => x.Id == id && x.CompanyId == companyId)
            .FirstOrDefaultAsync(cancellationToken);
}
