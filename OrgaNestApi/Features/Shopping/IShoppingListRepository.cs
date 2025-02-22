using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Shopping;

public interface IShoppingListRepository
{
    Task<ShoppingList?> GetByIdAsync(Guid id);
    Task<List<ShoppingList>> GetByUserIdAsync(Guid userId);
    Task AddAsync(ShoppingList shoppingList);
    Task UpdateAsync(ShoppingList shoppingList);
    Task DeleteAsync(Guid id);
}