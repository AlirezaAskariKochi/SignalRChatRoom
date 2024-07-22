public static class PaginationHelper
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int? pageNumber = null, int? pageSize = null)
    {
        // Default to first page and 10 items per page if no values provided
        pageNumber ??= 1;
        pageSize ??= 10000;

        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentException("Page size must be greater than or equal to 1.");
        }

        int skip = (pageNumber.Value - 1) * pageSize.Value;
        return query.Skip(skip).Take(pageSize.Value);
    }
}