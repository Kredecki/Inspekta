using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using MediatR;

namespace Inspekta.API.Queries.Users;

public sealed record DeleteUserCommand(Guid UserId) : IRequest;

public class DeleteUserCommandHandler(IUsersRepository usersRepository) : IRequestHandler<DeleteUserCommand>
{ 
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await usersRepository.GetUserById(request.UserId, cancellationToken) ?? 
                     throw new InvalidOperationException("E019");

        await usersRepository.DeleteUser(user, cancellationToken);
    }
}
