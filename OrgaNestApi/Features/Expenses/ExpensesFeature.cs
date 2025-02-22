using OrgaNestApi.Common.Domain;
using OrgaNestApi.Infrastructure.Database;

namespace OrgaNestApi.Features.Expenses;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Creates an expense with shared users.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto request)
    {
        var expense = await _expenseService.CreateExpenseAsync(
            request.UserId,
            request.FamilyId,
            request.Category,
            request.Amount,
            request.Date,
            request.Shares.Select(s => (s.UserId, s.Percentage)).ToList()
        );

        return CreatedAtAction(nameof(GetExpense), new { expenseId = expense.Id }, expense);
    }

    /// <summary>
    /// Gets expenses for a user.
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserExpenses(Guid userId)
    {
        var expenses = await _expenseService.GetUserExpensesAsync(userId);
        return Ok(expenses);
    }

    /// <summary>
    /// Gets expenses for a family.
    /// </summary>
    [HttpGet("family/{familyId}")]
    public async Task<IActionResult> GetFamilyExpenses(Guid familyId)
    {
        var expenses = await _expenseService.GetFamilyExpensesAsync(familyId);
        return Ok(expenses);
    }
    
    /// <summary>
    /// Gets a specific expense by its ID.
    /// </summary>
    [HttpGet("{expenseId}")]
    public async Task<IActionResult> GetExpense(Guid expenseId)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(expenseId);
        if (expense == null)
        {
            return NotFound();
        }
        return Ok(expense);
    }

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    [HttpPut("{expenseId}")]
    public async Task<IActionResult> UpdateExpense(Guid expenseId, [FromBody] UpdateExpenseDto request)
    {
        var updatedExpense = await _expenseService.UpdateExpenseAsync(
            expenseId,
            request.Category,
            request.Amount,
            request.Date,
            request.Shares.Select(s => (s.UserId, s.Percentage)).ToList()
        );

        if (updatedExpense == null)
        {
            return NotFound($"Expense with ID {expenseId} not found.");
        }

        return Ok(updatedExpense);
    }
    
    /// <summary>
    /// Deletes an expense by ID.
    /// </summary>
    [HttpDelete("{expenseId}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId)
    {
        var deleted = await _expenseService.DeleteExpenseAsync(expenseId);
        if (!deleted)
        {
            return NotFound($"Expense with ID {expenseId} not found.");
        }

        return NoContent();
    }    
}

public class ExpenseService: IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new expense with shared contributions.
    /// </summary>
    public async Task<ExpenseDto> CreateExpenseAsync(Guid userId, Guid? familyId, string categoryName, decimal amount, DateTime date, List<(Guid userId, decimal percentage)> shares)
    {
        if (shares.Any() && shares.Sum(s => s.percentage) != 1.0m)
        {
            throw new ArgumentException("Total share percentage must equal 100%.");
        }
        
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
        {
            throw new ArgumentException($"Category '{categoryName}' does not exist.");
        }

        var expense = new Expense.Builder()
            .SetUserId(userId)
            .SetFamilyId(familyId)
            .SetCategory(category)
            .SetAmount(amount)
            .SetDate(date)
            .AddExpenseShares(shares)
            .Build();

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense.ToDto();
    }

    /// <summary>
    /// Retrieves all expenses for a user.
    /// </summary>
    public async Task<List<ExpenseDto>> GetUserExpensesAsync(Guid userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        
        if (!userExists)
        {
            throw new InvalidOperationException($"User with id {userId} does not exist.");
        }
        
        return await _context.Expenses
            .Include(e => e.ExpenseShares)
            .Where(e => e.UserId == userId || e.ExpenseShares.Any(es => es.UserId == userId))
            .Select(e => e.ToDto())
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all expenses for a family.
    /// </summary>
    public async Task<List<ExpenseDto>> GetFamilyExpensesAsync(Guid familyId)
    {
        var familyExists = await _context.Families.AnyAsync(u => u.Id == familyId);
        
        if (!familyExists)
        {
            throw new InvalidOperationException($"Family with id {familyId} does not exist.");
        }
        
        return await _context.Expenses
            .Where(e => e.FamilyId == familyId)
            .Include(e => e.ExpenseShares)
            .Select(e => e.ToDto())
            .ToListAsync();
    }
    
    public async Task<ExpenseDto?> GetExpenseByIdAsync(Guid expenseId)
    {
        return await _context.Expenses
            .Include(e => e.ExpenseShares)
            .Select(e => e.ToDto())
            .FirstOrDefaultAsync(e => e.Id == expenseId);
    }
    
    public async Task<ExpenseDto?> UpdateExpenseAsync(Guid expenseId, string categoryName, decimal amount, DateTime date, List<(Guid userId, decimal percentage)> shares)
    {
        var existingExpense = await _context.Expenses
            .Include(e => e.ExpenseShares)
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (existingExpense == null)
        {
            throw new ArgumentException("Expense not found");
        }

        if (shares.Any() && shares.Sum(s => s.percentage) != 1.0m)
        {
            throw new ArgumentException("Total share percentage must equal 100%.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
        {
            throw new ArgumentException($"Category '{categoryName}' does not exist.");
        }

        // Create a new Expense instance with the updated values
        var updatedExpense = Expense.CreateBuilder()
            .SetUserId(existingExpense.UserId)
            .SetFamilyId(existingExpense.FamilyId)
            .SetCategory(category)
            .SetAmount(amount)
            .SetDate(date)
            .AddExpenseShares(shares)
            .Build();

        // Remove the old expense
        _context.ExpenseShares.RemoveRange(existingExpense.ExpenseShares);
        _context.Expenses.Remove(existingExpense);

        // Add the new one
        _context.Expenses.Add(updatedExpense);
        await _context.SaveChangesAsync();
    
        return updatedExpense.ToDto();
    }
    
    public async Task<bool> DeleteExpenseAsync(Guid expenseId)
    {
        var expense = await _context.Expenses
            .Include(e => e.ExpenseShares)
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (expense == null)
        {
            return false;
        }

        _context.ExpenseShares.RemoveRange(expense.ExpenseShares);
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }
    
}
