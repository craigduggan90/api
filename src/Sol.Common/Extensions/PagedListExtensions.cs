using Sol.Common.Pagination;

namespace Sol.Common.Extensions;

public static class PagedListExtensions
{
    public static PagedList<TTarget> Map<TSource, TTarget>(this PagedList<TSource> input, Func<TSource, TTarget> converter)
        => new(input.Data.Select(converter).ToList(), input.Cursor, input.Count);
}