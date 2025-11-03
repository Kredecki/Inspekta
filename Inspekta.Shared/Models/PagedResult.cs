using System.Text.Json.Serialization;

namespace Inspekta.Shared.Models;

public sealed class PagedResult<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; init; } = [];

    [JsonPropertyName("total")]
    public int Total { get; init; }
}
