using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Users;

public sealed record UpdateUserCommand(
    UserDto Dto) : IRequest<UserDto>;

public class UpdateUserCommandHandler(IUsersRepository usersRepository) : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await usersRepository.GetUserById(request.Dto.Id, cancellationToken) ?? throw new Exception("E018");
        await usersRepository.UpdateUser(user, cancellationToken);

        return request.Dto;
    }
}
