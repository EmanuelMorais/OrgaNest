using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Expenses;

public interface IExpenseService
{
    Task<Expense> CreateExpenseAsync(Guid userId, Guid? familyId, string category, decimal amount, DateTime date, List<(Guid userId, decimal percentage)> shares);
    Task<Expense?> GetExpenseByIdAsync(Guid expenseId);
    Task<List<Expense>> GetUserExpensesAsync(Guid userId);
    Task<List<Expense>> GetFamilyExpensesAsync(Guid familyId);
    Task<bool> DeleteExpenseAsync(Guid expenseId);

    Task<Expense?> UpdateExpenseAsync(Guid expenseId, string categoryName, decimal amount, DateTime date,
        List<(Guid userId, decimal percentage)> shares);
}
