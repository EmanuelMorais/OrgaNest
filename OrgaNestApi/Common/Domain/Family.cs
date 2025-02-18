namespace OrgaNestApi.Common.Domain;

public class Family
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    // Many-to-Many: A family can have multiple users
    public List<UserFamily> UserFamilies { get; set; } = new();

    // Expenses linked to the family
    public List<Expense> Expenses { get; set; } = new();
}