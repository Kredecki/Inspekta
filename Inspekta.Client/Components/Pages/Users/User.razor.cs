using Inspekta.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace Inspekta.Client.Components.Pages.Users;

public partial class User
{
    [Parameter]
    public Guid Id { get; set; }

    private UserDto Model { get; set; } = new();
}
