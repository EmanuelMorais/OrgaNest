namespace OrgaNestApi.Common.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Many-to-Many: A user can belong to multiple families
    public List<UserFamily> UserFamilies { get; set; } = new();

    // Expenses directly created by the user
    public List<Expense> Expenses { get; set; } = new();

    // Expenses the user is involved in (shared)
    public List<ExpenseShare> ExpenseShares { get; set; } = new();
    
    public List<ShoppingList>? ShoppingLists { get; set; }
}