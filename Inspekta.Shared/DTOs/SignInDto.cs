using Inspekta.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Inspekta.Shared.DTOs;

public class SignInDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public EUserRole Role { get; set; }
}
