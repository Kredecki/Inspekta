using Inspekta.Persistance.Entities;

namespace Inspekta.Persistance.Abstractions.Repositories;

public interface IUsersRepository
{
    public Task<List<User>> GetPagedUsers(int currentPage, int recordsPerPage, 
        CancellationToken cancellationToken = default);

    public Task<List<User>> GetPagedCompanyUsers(Guid companyId, int currentPage, int recordsPerPage,
        CancellationToken cancellationToken = default);

    public Task<User?> GetUserById(Guid id,
        CancellationToken cancellationToken = default);

    public Task<User?> GetCompanyUserById(Guid id, Guid companyId,
        CancellationToken cancellationToken = default);

    public Task UpdateUser(User user, Guid adminId,
        CancellationToken cancellationToken = default);

    public Task DeleteUser(User user,
        CancellationToken cancellationToken = default);
}
