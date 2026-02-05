using System.Linq.Expressions;
using System.Reflection;

namespace Inspekta.API.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Universal sorting by property path
    /// </summary>
    public static IQueryable<T> OrderByPath<T>(
        this IQueryable<T> source,
        string? sortColumn,
        bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortColumn))
            return source;

        string[] propertyPath = sortColumn.Split('.', StringSplitOptions.RemoveEmptyEntries);
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        Expression body = parameter;

        foreach (var part in propertyPath)
        {
            var type = body.Type;

            var prop = type.GetProperty(
                part,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop == null)
                return source;
            
            body = Expression.Property(body, prop);
        }

        Type propertyType = body.Type;

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
        LambdaExpression? lambda = Expression.Lambda(delegateType, body, parameter);

        string methodName = sortDescending ? "OrderByDescending" : "OrderBy";

        MethodInfo genericMethod = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == methodName &&
                m.GetParameters().Length == 2);

        MethodInfo method = genericMethod.MakeGenericMethod(typeof(T), propertyType);

        IQueryable<T> result = (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;

        return result;
    }
}
