using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Shopping;

public interface IShoppingListService
{
    Task<List<ShoppingList>> GetShoppingListsByUser(Guid userId);
    Task DeleteShoppingList(Guid id);
    Task UpdateShoppingList(UpdateShoppingListDto shoppingList);
    Task<ShoppingListDto> AddShoppingList(CreateShoppingListDto shoppingList);
    Task<ShoppingList?> GetShoppingListById(Guid id);
}