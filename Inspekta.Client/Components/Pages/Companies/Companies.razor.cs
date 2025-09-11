using Inspekta.Shared.DTOs;
using System.Net.Http.Json;

namespace Inspekta.Client.Components.Pages.Companies;

public partial class Companies
{
    private List<CompanyDto> Model { get; set; } = [];

    protected override async Task OnInitializedAsync()
	{
        Model = await GetData();
    }

	private async Task<List<CompanyDto>> GetData()
		=> await _HttpClient.GetFromJsonAsync<List<CompanyDto>>("Api/Companies/Get") ?? [];
}
