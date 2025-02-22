namespace OrgaNestApi.Features.Shopping;

public class CreateShoppingListDto
{
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<CreateShoppingItemDto> Items { get; set; } = new();
}

// DTO for updating an existing Shopping List
public class UpdateShoppingListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<UpdateShoppingItemDto> Items { get; set; } = new();
}

// DTO for creating a new Shopping Item
public class CreateShoppingItemDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsPurchased { get; set; }
}

// DTO for updating an existing Shopping Item
public class UpdateShoppingItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsPurchased { get; set; }
}

public class ShoppingListDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public List<ShoppingItemDto> Items { get; set; } = new();
}

public class ShoppingItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public bool IsPurchased { get; set; }
}