using OrgaNestApi.Features.Shopping;

namespace OrgaNestApi.Common.Domain;

public class ShoppingList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<ShoppingItem> Items { get; set; } = [];

    public ShoppingListDto ToDto()
    {
        return new ShoppingListDto
        {
            Id = Id,
            Name = Name,
            UserId = UserId,
            Items = Items.Select(item => item.ToDto()).ToList()
        };
    }
}