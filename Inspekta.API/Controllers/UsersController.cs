using Inspekta.API.Abstractions.Services;
using Inspekta.API.Queries.Users;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
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
        Guid sid = currentUserService.GetId(User);
        EUserRole userRole = currentUserService.GetRole(User);

        List<UserDto>? result = await mediator.Send(new GetUsersQuery(currentPage, recordsPerPage, userRole, sid), cancellationToken);

        if (result is not null)
            return Ok(result);

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
        Guid sid = currentUserService.GetId(User);
        EUserRole userRole = currentUserService.GetRole(User);

        UserDto? result = await mediator.Send(new GetUserByIdQuery(id, userRole, sid), cancellationToken);

        if (result is not null)
            return Ok(result);

        return BadRequest();
    }
}
