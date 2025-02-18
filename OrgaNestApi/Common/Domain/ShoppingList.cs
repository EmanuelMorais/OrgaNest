namespace OrgaNestApi.Common.Domain;

public class ShoppingList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public List<ShoppingItem> Items { get; set; } = new();
}