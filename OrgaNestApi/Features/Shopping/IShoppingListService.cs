using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Shopping;

public interface IShoppingListService
{
    Task<List<ShoppingList>> GetShoppingListsByUser(Guid userId);
    Task DeleteShoppingList(Guid id);
    Task UpdateShoppingList(ShoppingList shoppingList);
    Task AddShoppingList(ShoppingList shoppingList);
    Task<ShoppingList?> GetShoppingListById(Guid id);
}