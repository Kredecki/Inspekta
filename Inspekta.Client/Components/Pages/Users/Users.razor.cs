using Azure;
using Inspekta.Shared.DTOs;
using Inspekta.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Net.Http.Json;

namespace Inspekta.Client.Components.Pages.Users;

public partial class Users
{
    private PagedResult<UserDto> Model { get; set; } = new();
    private int CurrentPage { get; set; } = default;
    private int RecordsPerPage { get; set; } = 10;
    private int Pages { get; set; } = default;
    private string SearchTerm { get; set; } = string.Empty;
    private string CurrentSort { get; set; } = string.Empty;
    private bool SortDesc { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        Model = await GetData();
        Pages = CountPages();
    }

    private async Task<PagedResult<UserDto>> GetData(CancellationToken cancellationToken = default)
    {
        string? uri = QueryHelpers.AddQueryString("api/Users/GetPaged", new Dictionary<string, string?>
        {
            ["currentPage"] = CurrentPage.ToString(CultureInfo.InvariantCulture),
            ["recordsPerPage"] = RecordsPerPage.ToString(CultureInfo.InvariantCulture),
            ["searchTerm"] = SearchTerm,
            ["sortColumn"] = CurrentSort,
            ["sortDescending"] = SortDesc.ToString(CultureInfo.InvariantCulture),
        });

        HttpResponseMessage? response = await _HttpClient.GetAsync(uri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            InspektaError? error = await response.Content.ReadFromJsonAsync<InspektaError>(cancellationToken);

            if (error is not null)
                Toast.ShowError(T("get_data_failed") + T(error.Detail!));

            return new();
        }

        return await response.Content.ReadFromJsonAsync<PagedResult<UserDto>>(cancellationToken) ?? new();
    }

    private int CountPages()
        => Model.Total == 0 ? 0 : (Model.Total + RecordsPerPage - 1) / RecordsPerPage;

    private async Task OnSearchAsync(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;

        CurrentPage = 0;
        Model = await GetData();
        Pages = CountPages();
    }

    private async Task OnRecordsPerPageChangeAsync(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? "10";
        RecordsPerPage = int.TryParse(value, out var n) ? n : 10;

        CurrentPage = 0;
        Model = await GetData();
        Pages = CountPages();
    }

    private async Task SortBy(string column)
    {
        if (CurrentSort == column) SortDesc = !SortDesc;
        else { CurrentSort = column; SortDesc = false; }

        CurrentSort = column;

        CurrentPage = 0;
        Model = await GetData();
        Pages = CountPages();
    }

    private string GetSortIcon(string column)
        => CurrentSort == column
           ? (SortDesc ? "switch_left" : "switch_right")
           : "switch_right";

    private void Details(Guid id)
        => Navigation.NavigateTo($"/user/{id}?IsReadOnly=true");

    private async Task ChangePageAsync(int page)
    {
        CurrentPage = page;
        Model = await GetData();
    }

    private int GetRowNumber(int index)
        => CurrentPage * RecordsPerPage + index + 1;
}
