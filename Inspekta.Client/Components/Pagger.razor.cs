using Microsoft.AspNetCore.Components;

namespace Inspekta.Client.Components
{
    public partial class Pagger
    {
        [Parameter]
        public int CurrentPage { get; set; }

        [Parameter]
        public int Pages { get; set; }

        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }

        private async Task ChangePage(int page)
        {
            if (CurrentPageChanged.HasDelegate)
                await CurrentPageChanged.InvokeAsync(page);
        }
    }
}
