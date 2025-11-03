using Inspekta.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Inspekta.Shared.Models;

namespace Inspekta.Client.Components.Pages.Users;

public partial class Users
{
    private PagedResult<UserDto> Model { get; set; } = new();
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
        FilteredModel = Model.Items;

        Pages = Model.Total == 0 ? 0 : (Model.Total + RecordsPerPage - 1) / RecordsPerPage;
    }

    private async Task<PagedResult<UserDto>> GetData(CancellationToken cancellationToken = default)
    {
        string? uri = QueryHelpers.AddQueryString("api/Users/GetPaged", new Dictionary<string, string?>
        {
            ["currentPage"] = CurrentPage.ToString(CultureInfo.InvariantCulture),
            ["recordsPerPage"] = RecordsPerPage.ToString(CultureInfo.InvariantCulture)
        });

        HttpResponseMessage? resp = await _HttpClient.GetAsync(uri, cancellationToken);
        if (!resp.IsSuccessStatusCode) return new();

        return await resp.Content.ReadFromJsonAsync<PagedResult<UserDto>>(cancellationToken) ?? new();
    }

    private void CloseMenu() => OpenMenuForId = null;

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape") CloseMenu();
    }

    private async Task OnSearchAsync(ChangeEventArgs e)
    {
        SearchTerm = e.Value?.ToString() ?? string.Empty;

        var items = Model.Items ?? Enumerable.Empty<UserDto>();

        FilteredModel = items
            .Where(x =>
                (x.Login?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                x.Role.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private async Task OnRecordsPerPageChangeAsync(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? "10";
        RecordsPerPage = int.TryParse(value, out var n) ? n : 10;

        Model = await GetData();
        FilteredModel = Model.Items;
        Pages = Model.Total == 0 ? 0 : (Model.Total + RecordsPerPage - 1) / RecordsPerPage;
    }

    private void SortBy(SortColumn column)
    {
        if (CurrentSort == column) SortAsc = !SortAsc;
        else { CurrentSort = column; SortAsc = true; }

        IEnumerable<UserDto> filtr = FilteredModel;

        switch (column)
        {
            case SortColumn.Id:
                filtr = SortAsc
                    ? filtr.OrderBy(x => x.Id).ToList()
                    : filtr.OrderByDescending(x => x.Id).ToList();
                break;
            case SortColumn.Login:
                filtr = SortAsc
                    ? filtr.OrderBy(x => x.Login).ToList()
                    : filtr.OrderByDescending(x => x.Login).ToList();
                break;
            case SortColumn.Role:
                filtr = SortAsc
                    ? filtr.OrderBy(x => x.Role.ToString()).ToList()
                    : filtr.OrderByDescending(x => x.Role.ToString()).ToList();
                break;
        }

        FilteredModel = filtr.ToList();
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

    private async Task ChangePageAsync(int page)
    {
        CurrentPage = page;
        Model = await GetData();
        FilteredModel = Model.Items;
    }
}
