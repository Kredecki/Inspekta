using Microsoft.AspNetCore.Components;

namespace Inspekta.Client.Components
{
    public partial class TableHeader
    {
        [Parameter]
        public string SearchTerm { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> SearchTermChanged { get; set; }

        [Parameter]
        public int RecordsPerPage { get; set; }

        [Parameter]
        public EventCallback<int> RecordsPerPageChanged { get; set; }

        [Parameter]
        public EventCallback AddClicked { get; set; }

        [Parameter]
        public bool ShowAddButton { get; set; } = true;

        private async Task OnSearchInput(ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? string.Empty;
            await SearchTermChanged.InvokeAsync(value);
        }

        private async Task OnRecordsPerPageChanged(ChangeEventArgs e)
        {
            var value = int.TryParse(e.Value?.ToString(), out var n)
                ? n
                : 10;

            await RecordsPerPageChanged.InvokeAsync(value);
        }

        private async Task OnAddClicked()
        {
            await AddClicked.InvokeAsync();
        }
    }
}
