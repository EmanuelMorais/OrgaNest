namespace OrgaNestApi.Common.Domain;

public class ShoppingItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsPurchased { get; set; }
    public Guid ShoppingListId { get; set; }
    public ShoppingList ShoppingList { get; set; } = null!;
}