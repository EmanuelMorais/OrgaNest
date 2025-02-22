using OrgaNestApi.Features.Expenses;
using OrgaNestApi.Features.Users;
using ExpenseDto = OrgaNestApi.Features.Users.ExpenseDto;
using ExpenseShareDto = OrgaNestApi.Features.Users.ExpenseShareDto;

namespace OrgaNestApi.Common.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Many-to-Many: A user can belong to multiple families
    public List<UserFamily> UserFamilies { get; set; } = [];

    // Expenses directly created by the user
    public List<Expense> Expenses { get; set; } = [];

    // Expenses the user is involved in (shared)
    public List<ExpenseShare> ExpenseShares { get; set; } = [];
    
    public List<ShoppingList>? ShoppingLists { get; set; } = [];

    public UserDto ToDto()
    {
        return new UserDto
        {
            Id = Id,
            Name = Name,
            Email = Email,
            UserFamilies = this.UserFamilies.Select(u => new UserFamilyDto
            {
                UserId = u.UserId,
                FamilyId = u.FamilyId,
                FamilyName = u.Family?.Name
            }).ToList(),
            Expenses = this.Expenses.Select(e => new ExpenseDto
            {
                UserId = e.UserId,
                Date = e.Date,
                Amount = e.Amount,
                Category = e.Category?.Name,
                Shares = e.ExpenseShares.Select(x => new ExpenseShareDto
                {
                     UserId = x.UserId,
                     ExpenseId = x.ExpenseId,
                     Percentage = x.Percentage,
                }).ToList()
            }).ToList(),
            ShoppingLists = this.ShoppingLists.Select(s => new ShoppingListDto
            {
                
            }).ToList(),
            ExpenseShares = this.ExpenseShares.Select(e => new ExpenseShareDto
            {
                UserId = e.UserId,
                ExpenseId = e.ExpenseId,
                Percentage = e.Percentage,
                
            }).ToList()
        };
    }
}