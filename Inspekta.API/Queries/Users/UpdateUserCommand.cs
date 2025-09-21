using Inspekta.API.Abstractions.Services;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.DTOs;
using MediatR;

namespace Inspekta.API.Queries.Users;

public sealed record UpdateUserCommand(
    UserDto Dto,
    Guid AdminId) : IRequest<UserDto>;

public class UpdateUserCommandHandler(IUsersRepository usersRepository, IPasswordService passwordService) : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await usersRepository.GetUserById(request.Dto.Id, cancellationToken) ?? throw new Exception("E018");

        if(request.Dto.Password is null)
            throw new Exception("E022");

        string? salt = passwordService.GenerateNewSalt();
        string? hashedPassword = passwordService.HashPassword(request.Dto.Password, salt);

        if(request.Dto.Login is null)
            throw new Exception("E023");

        if(request.Dto.Company is null)
            throw new Exception("E024");

        if(request.Dto.Company.Name is null)
            throw new Exception("E025");

        user.Login = request.Dto.Login;
        user.PassHash = hashedPassword;
        user.Salt = salt;
        user.Role = request.Dto.Role;
        user.Company = new Company 
        { 
            Id = request.Dto.Company.Id,
            Name = request.Dto.Company.Name,
            NIP = request.Dto.Company.NIP,
            Street = request.Dto.Company.Street,
            ZipCode = request.Dto.Company.ZipCode,
            Town = request.Dto.Company.Town,
            Email = request.Dto.Company.Email,
            Phone = request.Dto.Company.Phone,
            CreatedAt = user.Company.CreatedAt,
            CreatedBy = user.Company.CreatedBy,
            ModifiedAt = user.Company.ModifiedAt,
            ModifiedBy = user.Company.ModifiedBy
        };

        await usersRepository.UpdateUser(user, request.AdminId, cancellationToken);

        return request.Dto;
    }
}
