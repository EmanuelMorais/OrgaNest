namespace OrgaNestApi.Common.Domain;

public class CursorPagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    
    public Guid? NextCursor { get; set; }
    
    public DateTime? NextCursorCreatedAt { get; set; }
}