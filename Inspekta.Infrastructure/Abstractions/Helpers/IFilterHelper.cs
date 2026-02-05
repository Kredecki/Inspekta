namespace Inspekta.Infrastructure.Abstractions.Helpers;

public interface IFilterHelper
{
    public Task<IEnumerable<T>> ApplySearchFilter<T>(
        IEnumerable<T> source,
        string searchTerm,
        Func<T, string?> searchFieldSelector);

    public Task<IEnumerable<T>> ApplySortFilter<T>(
        IEnumerable<T> source,
        string sortColumn,
        bool sortDescending);
}
