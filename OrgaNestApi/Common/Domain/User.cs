using System.ComponentModel.DataAnnotations;
using OrgaNestApi.Features.Users;
using ExpenseDto = OrgaNestApi.Features.Users.ExpenseDto;
using ExpenseShareDto = OrgaNestApi.Features.Users.ExpenseShareDto;

namespace OrgaNestApi.Common.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(150)] 
    public string Email { get; set; } = string.Empty;

    // Many-to-Many: A user can belong to multiple families
    public ICollection<UserFamily> UserFamilies { get; set; } = [];

    // Expenses directly created by the user
    public ICollection<Expense> Expenses { get; set; } = [];

    // Expenses the user is involved in (shared)
    public ICollection<ExpenseShare> ExpenseShares { get; set; } = [];

    public ICollection<ShoppingList>? ShoppingLists { get; set; } = [];

    public UserDto ToDto()
    {
        return new UserDto
        {
            Id = Id,
            Name = Name,
            Email = Email,
            UserFamilies = UserFamilies.Select(u => new UserFamilyDto
            {
                UserId = u.UserId,
                FamilyId = u.FamilyId,
                FamilyName = u.Family.Name
            }).ToList(),
            Expenses = Expenses.Select(e => new ExpenseDto
            {
                UserId = e.UserId,
                Date = e.Date,
                Amount = e.Amount,
                Category = e.Category.Name,
                Shares = e.ExpenseShares.Select(x => new ExpenseShareDto
                {
                    UserId = x.UserId,
                    ExpenseId = x.ExpenseId,
                    Percentage = x.Percentage
                }).ToList()
            }).ToList(),
            ShoppingLists = ShoppingLists.Select(s => new ShoppingListDto
            {
                Id = s.Id,
                Name = s.Name,
                UserId = s.UserId,
                Items = s.Items.Select(i => new ShoppingItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    IsPurchased = i.IsPurchased
                }).ToList()
            }).ToList(),
            ExpenseShares = ExpenseShares.Select(e => new ExpenseShareDto
            {
                UserId = e.UserId,
                ExpenseId = e.ExpenseId,
                Percentage = e.Percentage
            }).ToList()
        };
    }
}