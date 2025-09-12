using Inspekta.Shared.Enums;

namespace Inspekta.Shared.DTOs;

public sealed class UserDto
{
	public Guid Id { get; set; } = Guid.Empty;
	public string? Login { get; set; }
	public string? Password { get; set; }
	public string? Token { get; set; }
	public EUserRole Role { get; set; }
	public CompanyDto? Company { get; set; }
}
