namespace OrgaNestApi.Features.Expenses;

public interface IExpenseService
{
    Task<ExpenseDto> CreateExpenseAsync(Guid userId, Guid? familyId, string category, decimal amount, DateTime date,
        List<(Guid userId, decimal percentage)> shares);

    Task<ExpenseDto?> GetExpenseByIdAsync(Guid expenseId);
    Task<List<ExpenseDto>> GetUserExpensesAsync(Guid userId);
    Task<List<ExpenseDto>> GetFamilyExpensesAsync(Guid familyId);
    Task<bool> DeleteExpenseAsync(Guid expenseId);

    Task<ExpenseDto?> UpdateExpenseAsync(Guid expenseId, string categoryName, decimal amount, DateTime date,
        List<(Guid userId, decimal percentage)> shares);
}