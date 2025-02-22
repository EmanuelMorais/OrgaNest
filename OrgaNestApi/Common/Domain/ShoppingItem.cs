using System.Text.Json.Serialization;
using OrgaNestApi.Features.Shopping;

namespace OrgaNestApi.Common.Domain;

public class ShoppingItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public bool IsPurchased { get; set; }
    public Guid ShoppingListId { get; set; }

    [JsonIgnore] public ShoppingList ShoppingList { get; set; } = null!;

    public ShoppingItemDto ToDto()
    {
        return new ShoppingItemDto
        {
            Id = Id,
            Name = Name,
            Quantity = Quantity,
            IsPurchased = IsPurchased
        };
    }
}