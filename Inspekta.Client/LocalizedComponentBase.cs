using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Inspekta.Client;

public class LocalizedComponentBase : ComponentBase
{
    [Inject]
    private IStringLocalizerFactory LocalizerFactory { get; set; } = default!;

    private IStringLocalizer? _localizer;

    protected IStringLocalizer L =>
        _localizer ??= LocalizerFactory.Create(GetType());

    protected string T(string key) => L[key];
}
