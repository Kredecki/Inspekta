using Inspekta.Shared.Enums;

namespace Inspekta.Shared.DTOs;

public sealed class UserDto
{
	public Guid Id { get; set; } = Guid.Empty;
	public string Login { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string Token { get; set; } = string.Empty;
	public EUserRole Role { get; set; } = EUserRole.Diagnosta;
}
