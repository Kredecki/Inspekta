using Inspekta.Infrastructure.Abstractions.Helpers;
using System.Reflection;

namespace Inspekta.Infrastructure.Helpers;

public class FilterHelper : IFilterHelper
{
    public async Task<IEnumerable<T>> ApplySearchFilter<T>(
        IEnumerable<T> source,
        string searchTerm,
        Func<T, string?> searchFieldSelector)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLower();

            source = source.Where(x =>
            {
                string? field = searchFieldSelector(x);
                return field is not null &&
                       field.Contains(term, StringComparison.OrdinalIgnoreCase);
            });
        }

        return source;
    }

    public async Task<IEnumerable<T>> ApplySortFilter<T>(
        IEnumerable<T> source,
        string sortColumn,
        bool sortDescending)
    {
        if (!string.IsNullOrWhiteSpace(sortColumn) && sortColumn != "None")
        {
            PropertyInfo? prop = typeof(T).GetProperty(sortColumn);
            if (prop == null) return source;

            return sortDescending
                ? source.OrderByDescending(x => prop.GetValue(x, null))
                : source.OrderBy(x => prop.GetValue(x, null));
        }

        return source;
    }
}
