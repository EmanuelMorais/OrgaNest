using OrgaNestApi.Features.Shopping;

namespace OrgaNestApi.Common.Domain;

public class ShoppingList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public List<ShoppingItem> Items { get; set; } = new();
    
    public ShoppingListDto ToDto()
    {
        return new ShoppingListDto
        {
            Id = this.Id,
            Name = this.Name,
            UserId = this.UserId,
            Items = this.Items.Select(item => item.ToDto()).ToList()
        };
    }
}