namespace OrgaNestApi.Common.Domain;

public class Family
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    // Many-to-Many: A family can have multiple users
    public ICollection<UserFamily> UserFamilies { get; set; } = [];

    // Expenses linked to the family
    public ICollection<Expense> Expenses { get; set; } = [];
}