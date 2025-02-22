using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Expenses;
using OrgaNestApi.Features.Shopping;

namespace OrgaNestApi.Features.Users;

// DTO for creating a user
public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// DTO for getting user details (you can expand this as needed)
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public List<UserFamilyDto> UserFamilies { get; set; } = [];

    public List<ExpenseDto> Expenses { get; set; } = [];

    public List<ExpenseShareDto> ExpenseShares { get; set; } = [];
    
    public List<ShoppingListDto>? ShoppingLists { get; set; } = [];
}
public class ExpenseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public Guid? FamilyId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    
    public List<ExpenseShareDto> Shares { get; set; } = new();
}

public class ExpenseShareDto
{
    public Guid UserId { get; set; }

    public Guid ExpenseId { get; set; }

    public decimal Percentage { get; set; }
}

public class UserFamilyDto
{
    public Guid UserId { get; set; }
    public Guid FamilyId { get; set; }
    public string FamilyName { get; set; } = string.Empty;
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