using Microsoft.AspNetCore.Components;

namespace Inspekta.Client.Components.Layout.MainLayout;

public partial class Application
{
	[Parameter]
	public string Name { get; set; } = string.Empty;
}
