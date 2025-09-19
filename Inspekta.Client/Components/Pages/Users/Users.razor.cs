using Inspekta.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Inspekta.Client.Components.Pages.Users;

public partial class Users
{
    private List<UserDto> Model { get; set; } = [];
    private List<UserDto> FilteredModel { get; set; } = [];
    private Guid? OpenMenuForId { get; set; }
    private string SearchTerm { get; set; } = string.Empty;
    private int CurrentPage { get; set; } = default;
    private int Pages { get; set; } = default;
    private int RecordsPerPage { get; set; } = 10;
    private enum SortColumn { None, Id, Login, Role }
    private SortColumn CurrentSort { get; set; } = SortColumn.None;
    private bool SortAsc { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        Model = await GetData();
        FilteredModel = Model;
    }

    private async Task<List<UserDto>> GetData(CancellationToken cancellationToken = default)
    {
        string? uri = QueryHelpers.AddQueryString("api/Users/GetPaged", new Dictionary<string, string?>
        {
            ["currentPage"] = CurrentPage.ToString(CultureInfo.InvariantCulture),
            ["recordsPerPage"] = RecordsPerPage.ToString(CultureInfo.InvariantCulture)
        });

        HttpResponseMessage? resp = await _HttpClient.GetAsync(uri, cancellationToken);
        if (!resp.IsSuccessStatusCode) return [];

        return await resp.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken) ?? [];
    }

    private void CloseMenu() => OpenMenuForId = null;

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape") CloseMenu();
    }

    private Task OnSearchAsync(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;
        FilteredModel = Model.Where(x => x.Login.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        CurrentPage = 0;
        RecalcPagesAndClamp();
        return Task.CompletedTask;
    }

    private void RecalcPagesAndClamp()
    {
        Pages = Math.Max(1, (int)Math.Ceiling((double)FilteredModel.Count / Math.Max(1, RecordsPerPage)));
        CurrentPage = Math.Clamp(CurrentPage, 0, Pages - 1);
        StateHasChanged();
    }

    private Task OnRecordsPerPageChangeAsync(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? "10";
        RecordsPerPage = int.TryParse(value, out var n) ? n : 10;
        RecalcPagesAndClamp();
        return Task.CompletedTask;
    }

    private void ApplyFilterAndSort()
    {
        IEnumerable<UserDto> q = Model;

        if (!string.IsNullOrWhiteSpace(SearchTerm))
            q = q.Where(x => x.Login.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        q = CurrentSort switch
        {
            SortColumn.Id => (SortAsc ? q.OrderBy(x => x.Id) : q.OrderByDescending(x => x.Id)),
            SortColumn.Login => (SortAsc ? q.OrderBy(x => x.Login) : q.OrderByDescending(x => x.Login)),
            SortColumn.Role => (SortAsc ? q.OrderBy(x => x.Role) : q.OrderByDescending(x => x.Role)),
            _ => q
        };

        FilteredModel = q.ToList();
        RecalcPagesAndClamp();
    }

    private void SortBy(SortColumn column)
    {
        if (CurrentSort == column) SortAsc = !SortAsc;
        else { CurrentSort = column; SortAsc = true; }

        CurrentPage = 0;
        ApplyFilterAndSort();
    }

    private string GetSortIcon(SortColumn column)
        => CurrentSort == column
           ? (SortAsc ? "switch_left" : "switch_right")
           : "switch_right";

    private void Details(Guid id)
    {
        CloseMenu();
        Navigation.NavigateTo($"/user/{id}?IsReadOnly=true");
    }

    private Task ChangePageAsync(int page)
    {
        CurrentPage = Math.Clamp(page, 0, Math.Max(Pages - 1, 0));
        return Task.CompletedTask;
    }
}
