using System.ComponentModel.DataAnnotations;

namespace Inspekta.Shared.DTOs.User;

public class UserCreateDto
{
    [Required(ErrorMessage = "Login nie może być pusty.")]
    public string? Login { get; set; }

    [Required(ErrorMessage = "Hasło nie może być puste.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Hasło nie może być puste."),
    Compare(nameof(Password), ErrorMessage = "Hasła muszą być identyczne.")]
    public string? ConfirmPassword { get; set; }
}
