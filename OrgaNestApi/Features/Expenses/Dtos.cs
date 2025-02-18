namespace OrgaNestApi.Features.Expenses;

/// <summary>
/// DTO for creating an expense.
/// </summary>
public class CreateExpenseDto
{
    public Guid UserId { get; set; }
    public Guid? FamilyId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public List<ExpenseShareDto> Shares { get; set; } = new();
}

/// <summary>
/// DTO for expense shares.
/// </summary>
public class ExpenseShareDto
{
    public Guid UserId { get; set; }
    public decimal Percentage { get; set; }
}

public class UpdateExpenseDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public List<ExpenseShareDto> Shares { get; set; } = new();
}
