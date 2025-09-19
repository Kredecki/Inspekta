using Inspekta.API.Queries.Companies;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inspekta.API.Controllers;

/// <inheritdoc />
[Route("Api/[controller]")]
[ApiController]
public class CompaniesController(IMediator mediator) : ControllerBase
{
	/// <summary>
	/// GetCompanies endpoint.
	/// </summary>
	/// <remarks>
	///     GET /Api/Companies/Get
	/// </remarks>
	/// <response code="200">Returns list of companies models</response>
	/// <response code="400">List of companies models are null or empty</response>
	[Authorize(Roles = $"{nameof(EUserRole.SuperAdministrator)}")]
	[HttpGet("GetPaged")]
	[ProducesResponseType(200, Type = typeof(List<CompanyDto>))]
	[ProducesResponseType(400, Type = typeof(ProblemDetails))]
	[Produces("application/json")]
	public async Task<IActionResult> GetPagedCompanies([FromQuery] int currentPage = 0, [FromQuery] int recordsPerPage = 10, 
		CancellationToken cancellationToken = default)
	{
		List<CompanyDto>? result = await mediator.Send(new GetCompaniesQuery(currentPage, recordsPerPage), cancellationToken);

		if (result is not null)
			return Ok(result);

		return BadRequest();
	}
}
