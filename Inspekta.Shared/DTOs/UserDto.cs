using Inspekta.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Inspekta.Shared.DTOs;

public sealed class UserDto
{
	public Guid Id { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Login nie może być pusty.")] 
	public string? Login { get; set; }

    [Required(ErrorMessage = "Hasło nie może być puste.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Hasło nie może być puste."), 
    Compare(nameof(Password), ErrorMessage = "Hasła muszą być identyczne.")]
    public string? ConfirmPassword { get; set; }

    public string? Token { get; set; }

	public EUserRole Role { get; set; }
	public CompanyDto? Company { get; set; }
}
