using Inspekta.API.Abstractions.Services;
using Inspekta.API.Queries.Users;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using Inspekta.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inspekta.API.Controllers;

/// <inheritdoc />
[Route("Api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// GetPagedUsers endpoint.
    /// </summary>
    /// <remarks>
    ///     GET /Api/Users/GetPaged
    /// </remarks>
    /// <response code="200">Returns list of users models</response>
    /// <response code="400">List of users models are null or empty</response>
    [Authorize(Roles = $"{nameof(EUserRole.Administrator)}, {nameof(EUserRole.SuperAdministrator)}")]
    [HttpGet("GetPaged")]
    [ProducesResponseType(200, Type = typeof(List<UserDto>))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [Produces("application/json")]
    public async Task<IActionResult> GetPagedUsers([FromQuery] int currentPage = 0, [FromQuery] int recordsPerPage = 10, 
        CancellationToken cancellationToken = default)
    {
        Guid sid = currentUserService.GetId();
        EUserRole userRole = currentUserService.GetRole();

        (List<UserDto> items, int total) = await mediator.Send(new GetUsersQuery(currentPage, recordsPerPage, userRole, sid), cancellationToken);

        if (items is not null)
        {
            PagedResult<UserDto> result = new()
            {
                Items = items,
                Total = total
            };

            return Ok(result);
        }

        return BadRequest();
    }

    /// <summary>
    /// GetUserById endpoint.
    /// </summary>
    /// <remarks>
    ///     GET /Api/Users/GetById
    /// </remarks>
    /// <response code="200">Returns user model</response>
    /// <response code="400">User model are null or empty</response>
    [Authorize(Roles = $"{nameof(EUserRole.Administrator)}, {nameof(EUserRole.SuperAdministrator)}")]
    [HttpGet("GetById")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [Produces("application/json")]
    public async Task<IActionResult> GetUserById([FromQuery] Guid id, 
        CancellationToken cancellationToken = default)
    {
        Guid sid = currentUserService.GetId();
        EUserRole userRole = currentUserService.GetRole();

        UserDto? result = await mediator.Send(new GetUserByIdQuery(id, userRole, sid), cancellationToken);

        if (result is not null)
            return Ok(result);

        return BadRequest();
    }

    /// <summary>
    /// UpdateUser endpoint.
    /// </summary>
    /// <remarks>
    ///     GET /Api/Users/Update
    /// </remarks>
    /// <response code="200">Returns updated user model</response>
    /// <response code="400">Error while updating user</response>
    [Authorize(Roles = $"{nameof(EUserRole.Administrator)}, {nameof(EUserRole.SuperAdministrator)}")]
    [HttpPut("Update")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [Produces("application/json")]
    public async Task<IActionResult> UpdateUser(UserDto user,
        CancellationToken cancellationToken = default)
    {
        Guid sid = currentUserService.GetId();

        UserDto? result = await mediator.Send(new UpdateUserCommand(user, sid), cancellationToken);

        if (result is not null)
            return Ok(result);

        return BadRequest();
    }

    /// <summary>
    /// DeleteUser endpoint.
    /// </summary>
    /// <remarks>
    ///     GET /Api/Users/Delete
    /// </remarks>
    /// <response code="200">Returns OK</response>
    /// <response code="400">Error while deleting user</response>
    [Authorize(Roles = $"{nameof(EUserRole.Administrator)}, {nameof(EUserRole.SuperAdministrator)}")]
    [HttpDelete("Delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return Ok();
    }
}
