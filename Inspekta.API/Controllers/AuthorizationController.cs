using Inspekta.API.Abstractions.Services;
using Inspekta.API.Queries.Authorization;
using Inspekta.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inspekta.API.Controllers
{
	/// <inheritdoc />
	[Route("Api/[controller]")]
	[ApiController]
	public class AuthorizationController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
	{
		/// <summary>
		/// User SignUp endpoint.
		/// </summary>
		/// <remarks>
		///     POST /Api/Authorization/SignUp \
		///     { \
		///         "id": "USER IDENTIFIER", \
		///         "login": "LOGIN" \
		///         "password": "USER PASSWORD" \
		///         "token": "TOKEN" \
		///     }
		/// </remarks>
		/// <response code="200">Returns user's model</response>
		/// <response code="400">Credentials are incorrect, signup failed</response>
		[Authorize]
		[HttpPost("SignUp")]
		[ProducesResponseType(200, Type = typeof(UserDto))]
		[ProducesResponseType(400, Type = typeof(ProblemDetails))]
		[Produces("application/json")]
		public async Task<IActionResult> SignUp(UserDto dto, CancellationToken cancellationToken = default)
		{
			Guid sid = currentUserService.GetId();

            UserDto? result = await mediator.Send(new SignUpQuery(dto, sid), cancellationToken);

			if (result is not null)
				return Ok(result);

			return BadRequest();
		}

		/// <summary>
		/// User SignIn endpoint.
		/// </summary>
		/// <remarks>
		///     POST /Api/Authorization/SignIn \
		///     { \
		///         "id": "USER IDENTIFIER", \
		///         "login": "LOGIN" \
		///         "password": "USER PASSWORD" \
		///         "token": "TOKEN" \
		///     }
		/// </remarks>
		/// <response code="200">Returns authenticated user's model</response>
		/// <response code="400">Credentials are incorrect, auth failed</response>
		[AllowAnonymous]
		[HttpPost("SignIn")]
		[ProducesResponseType(200, Type = typeof(UserDto))]
		[ProducesResponseType(400, Type = typeof(ProblemDetails))]
		[Produces("application/json")]
		public async Task<IActionResult> SignIn(SignInDto dto, CancellationToken cancellationToken = default)
		{
			SignInDto? result = await mediator.Send(new SignInQuery(dto), cancellationToken);

			if (result is not null)
				return Ok(result);

			return BadRequest();
		}
	}
}
