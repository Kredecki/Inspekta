using Inspekta.API.Abstractions.Services;
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
public class CompaniesController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
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

	/// <summary>
	/// GetCompanyById endpoint.
	/// </summary>
	/// <remarks>
	///     GET /Api/Companies/GetById
	/// </remarks>
	/// <response code="200">Returns company model</response>
	/// <response code="400">Company model are null or empty</response>
	[Authorize(Roles = $"{nameof(EUserRole.SuperAdministrator)}")]
	[HttpGet("GetById")]
	[ProducesResponseType(200, Type = typeof(CompanyDto))]
	[ProducesResponseType(400, Type = typeof(ProblemDetails))]
	[Produces("application/json")]
	public async Task<IActionResult> GetCompanyById([FromQuery] Guid id, 
		CancellationToken cancellationToken = default)
	{
		CompanyDto? result = await mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);

		if (result is not null)
			return Ok(result);

		return BadRequest();
    }

	/// <summary>
	/// CreateCompany endpoint.
	/// </summary>
	/// <remarks>
	///     GET /Api/Companies/Create
	/// </remarks>
	/// <response code="200">Returns created user model</response>
	/// <response code="400">Error while creating user</response>
	[Authorize(Roles = $"{nameof(EUserRole.SuperAdministrator)}")]
	[HttpPost("Create")]
	[ProducesResponseType(200, Type = typeof(CompanyDto))]
	[ProducesResponseType(400, Type = typeof(ProblemDetails))]
	[Produces("application/json")]
	public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companyDto, 
		CancellationToken cancellationToken = default)
	{
        Guid sid = currentUserService.GetId();

        CompanyDto? result = await mediator.Send(new CreateCompanyCommand(companyDto, sid), cancellationToken);

		if (result is not null)
			return Ok(result);

		return BadRequest();
    }

	/// <summary>
	/// UpdateCompany endpoint.
	/// </summary>
	/// <remarks>
	///     GET /Api/Companies/Update
	/// </remarks>
	/// <response code="200">Returns updated company model</response>
	/// <response code="400">Error while updating company</response>
	[Authorize(Roles = $"{nameof(EUserRole.SuperAdministrator)}")]
	[HttpPut("Update")]
	[ProducesResponseType(200, Type = typeof(CompanyDto))]
	[ProducesResponseType(400, Type = typeof(ProblemDetails))]
	[Produces("application/json")]
	public async Task<IActionResult> UpdateCompany([FromBody] CompanyDto companyDto, 
		CancellationToken cancellationToken = default)
	{
		Guid sid = currentUserService.GetId();

		CompanyDto? result = await mediator.Send(new UpdateCompanyCommand(companyDto, sid), cancellationToken);

		if (result is not null)
			return Ok(result);

		return BadRequest();
    }

	/// <summary>
	/// DeleteCompany endpoint.
	/// </summary>
	/// <remarks>
	///     GET /Api/Companies/Delete
	/// </remarks>
	/// <response code="200">Returns OK</response>
	/// <response code="400">Error while deleting company</response>
	[Authorize(Roles = $"{nameof(EUserRole.SuperAdministrator)}")]
	[HttpDelete("Delete")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400, Type = typeof(ProblemDetails))]
	[Produces("application/json")]
    public async Task<IActionResult> DeleteCompany([FromQuery] Guid id, 
		CancellationToken cancellationToken = default)
	{
		await mediator.Send(new DeleteCompanyCommand(id), cancellationToken);
		return Ok();
    }
}
