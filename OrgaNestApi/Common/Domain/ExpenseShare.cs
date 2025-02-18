namespace OrgaNestApi.Common.Domain;

public class ExpenseShare
{
    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal Percentage { get; set; } // Example: 70% = 0.7
}