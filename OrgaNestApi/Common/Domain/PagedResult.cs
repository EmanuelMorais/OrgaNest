namespace OrgaNestApi.Common.Domain;

public class PagedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
}
