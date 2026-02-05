using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance.Repositories;

public class UsersRepository(ApplicationDbContext dbContext) : IUsersRepository
{
    public async Task<IEnumerable<User>> GetUsers(
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = dbContext.Users
            .AsNoTracking();

        return await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

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
