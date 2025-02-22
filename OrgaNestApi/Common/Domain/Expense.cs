using OrgaNestApi.Features.Expenses;

namespace OrgaNestApi.Common.Domain;

public partial class Expense
{
    private Expense()
    {
    }

    public Expense(
        Guid userId,
        Guid? familyId,
        Category category,
        decimal amount,
        DateTime date,
        List<ExpenseShare> shares)
    {
        UserId = userId;
        FamilyId = familyId;
        Category = category;
        Amount = amount;
        Date = date;
        ExpenseShares = shares;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public Guid? FamilyId { get; init; }
    public Family? Family { get; init; }
    public Guid CategoryId { get; set; }
    public Category Category { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public List<ExpenseShare> ExpenseShares { get; init; } = new();

    public static Builder CreateBuilder()
    {
        return new Builder();
    }

    // public void AddShares(IEnumerable<ExpenseShare> shares)
    // {
    //     if (shares.Any() && shares.Sum(s => s.Percentage) != 1.0m)
    //     {
    //         throw new ArgumentException("Total share percentage must equal 100%.");
    //     }
    //
    //     ExpenseShares.AddRange(shares);
    // }

    public ExpenseDto ToDto()
    {
        return new ExpenseDto
        {
            Id = Id,
            UserId = UserId,
            FamilyId = FamilyId,
            Category = Category?.Name,
            Amount = Amount,
            Date = Date,
            Shares = ExpenseShares.Select(e => e.ToDto()).ToList()
        };
    }
}

public partial class Expense
{
    public class Builder
    {
        private readonly List<ExpenseShare> _expenseShares;
        private decimal _amount;
        private Category _category;
        private DateTime _date;
        private Guid? _familyId;
        private Guid _userId;

        public Builder()
        {
            _expenseShares = new List<ExpenseShare>(); // Ensuring fresh list for each new expense
        }

        public Builder SetUserId(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public Builder SetFamilyId(Guid? familyId)
        {
            _familyId = familyId;
            return this;
        }

        public Builder SetCategory(Category category)
        {
            _category = category;
            return this;
        }

        public Builder SetAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public Builder SetDate(DateTime date)
        {
            _date = date;
            return this;
        }

        // Add expense shares with validation
        public Builder AddExpenseShares(IEnumerable<(Guid userId, decimal percentage)> shares)
        {
            if (shares.Any() && shares.Sum(s => s.percentage) != 1.0m)
                throw new ArgumentException("Total share percentage must equal 100%.");

            foreach (var share in shares)
                _expenseShares.Add(new ExpenseShare
                {
                    UserId = share.userId,
                    Percentage = share.percentage
                });

            return this;
        }

        public Expense Build()
        {
            if (_userId == Guid.Empty)
                throw new ArgumentException("UserId must be set.");
            if (_category == null)
                throw new ArgumentException("Category must be set.");
            if (_amount == 0)
                throw new ArgumentException("Amount must be set and non-zero.");
            if (_date == default)
                throw new ArgumentException("Date must be set.");

            return new Expense(_userId, _familyId, _category, _amount, _date, _expenseShares);
        }
    }
}