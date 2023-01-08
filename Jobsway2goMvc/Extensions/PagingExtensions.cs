namespace Jobsway2goMvc.Extensions;

public static class PagingExtensions
{
    public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int pageSize, int page)
    {
        return items.Skip((page-1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> Page<T>(this IQueryable<T> items, int pageSize, int page)
    {
        return items.Skip((page-1) * pageSize).Take(pageSize);
    }
}
