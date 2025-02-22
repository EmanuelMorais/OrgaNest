namespace OrgaNestApi.Features.Shopping;
using OrgaNestApi.Common.Domain;

    public static class ShoppingListExtensions
    {
        // Convert ShoppingList to CreateShoppingListDto
        public static CreateShoppingListDto ToDtoForCreate(this ShoppingList shoppingList)
        {
            return new CreateShoppingListDto
            {
                Name = shoppingList.Name,
                UserId = shoppingList.UserId,
                Items = shoppingList.Items.Select(item => item.ToDtoForCreate()).ToList()
            };
        }

        // Convert ShoppingList to UpdateShoppingListDto
        public static UpdateShoppingListDto ToDtoForUpdate(this ShoppingList shoppingList)
        {
            return new UpdateShoppingListDto
            {
                Id = shoppingList.Id,
                Name = shoppingList.Name,
                UserId = shoppingList.UserId,
                Items = shoppingList.Items.Select(item => item.ToDtoForUpdate()).ToList()
            };
        }

        // Convert CreateShoppingListDto to domain ShoppingList
        public static ShoppingList ToDomain(this CreateShoppingListDto shoppingListDto)
        {
            return new ShoppingList
            {
                Name = shoppingListDto.Name,
                UserId = shoppingListDto.UserId,
                Items = shoppingListDto.Items.Select(item => item.ToDomain()).ToList()
            };
        }

        // Convert UpdateShoppingListDto to domain ShoppingList
        public static ShoppingList ToDomain(this UpdateShoppingListDto shoppingListDto)
        {
            return new ShoppingList
            {
                Id = shoppingListDto.Id,
                Name = shoppingListDto.Name,
                UserId = shoppingListDto.UserId,
                Items = shoppingListDto.Items.Select(item => item.ToDomain()).ToList()
            };
        }
    }

    public static class ShoppingItemExtensions
    {
        // Convert ShoppingItem to CreateShoppingItemDto
        public static CreateShoppingItemDto ToDtoForCreate(this ShoppingItem shoppingItem)
        {
            return new CreateShoppingItemDto
            {
                Name = shoppingItem.Name,
                Quantity = shoppingItem.Quantity,
                IsPurchased = shoppingItem.IsPurchased
            };
        }

        // Convert ShoppingItem to UpdateShoppingItemDto
        public static UpdateShoppingItemDto ToDtoForUpdate(this ShoppingItem shoppingItem)
        {
            return new UpdateShoppingItemDto
            {
                Id = shoppingItem.Id,
                Name = shoppingItem.Name,
                Quantity = shoppingItem.Quantity,
                IsPurchased = shoppingItem.IsPurchased
            };
        }

        // Convert CreateShoppingItemDto to domain ShoppingItem
        public static ShoppingItem ToDomain(this CreateShoppingItemDto shoppingItemDto)
        {
            return new ShoppingItem
            {
                Name = shoppingItemDto.Name,
                Quantity = shoppingItemDto.Quantity,
                IsPurchased = shoppingItemDto.IsPurchased
            };
        }

        // Convert UpdateShoppingItemDto to domain ShoppingItem
        public static ShoppingItem ToDomain(this UpdateShoppingItemDto shoppingItemDto)
        {
            return new ShoppingItem
            {
                Id = shoppingItemDto.Id,
                Name = shoppingItemDto.Name,
                Quantity = shoppingItemDto.Quantity,
                IsPurchased = shoppingItemDto.IsPurchased
            };
        }
    }