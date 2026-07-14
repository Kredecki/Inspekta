using Inspekta.Shared.DTOs;
using Inspekta.Shared.DTOs.User;
using Inspekta.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Inspekta.Client.Components.Pages.Users;

public partial class User
{
    [Parameter]
    public Guid Id { get; set; }
    [Parameter]
    public bool IsReadOnly { get; set; }

    private UserDto Model { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        CancellationToken ct = new();
        HttpResponseMessage? response = await _HttpClient.PostAsJsonAsync<UserDto>("Api/Authorization/SignUp", Model, ct);

        if (response is null)
            return;

        if (response.IsSuccessStatusCode)
        {
            UserDto? result = await response.Content.ReadFromJsonAsync<UserDto>(ct);
            if (result is null) return;

            Toast.ShowSuccess(T("user_created_successfully"));
            Navigation.Refresh();
        }
        else
        {
            InspektaError? error = await response.Content.ReadFromJsonAsync<InspektaError>(ct);

            if (error is not null)
                Toast.ShowError(T("creating_user_failed") + T(error.Detail!));
        }
    }

    private void HandleInvalidSubmit()
    {
        Console.WriteLine("Form submission failed due to validation errors.");
        // Handle form submission failure logic here
    }
}
