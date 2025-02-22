using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = new PagedResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = await query.CountAsync(cancellationToken),
            Data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
        };

        return result;
    }
}