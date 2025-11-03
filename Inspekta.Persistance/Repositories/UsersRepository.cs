using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Inspekta.Persistance.Repositories;

public class UsersRepository(ApplicationDbContext dbContext) : IUsersRepository
{
    public async Task<List<User>> GetUsers(CancellationToken cancellationToken = default)
        => await dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<User>> GetPagedUsers(int currentPage, int recordsPerPage, 
        CancellationToken cancellationToken = default)
        => await dbContext.Users
            .Skip(currentPage * recordsPerPage)
            .Take(recordsPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<User>> GetCompanyUsers(Guid companyId, 
        CancellationToken cancellationToken = default)
        => await dbContext.Users
             .Where(u => u.CompanyId == companyId)
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

    public async Task UpdateUser(User user, Guid adminId,
        CancellationToken cancellationToken = default)
    {
        user.ModifiedBy = adminId;
        user.ModifiedAt = DateTime.Now;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUser(User user, 
        CancellationToken cancellationToken = default)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
