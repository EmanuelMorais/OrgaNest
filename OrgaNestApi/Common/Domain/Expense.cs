namespace OrgaNestApi.Common.Domain;

public class ExpenseBuilder
{
    private Guid _userId;
    private Guid? _familyId;
    private Category _category;
    private decimal _amount;
    private DateTime _date;
    private List<ExpenseShare> _expenseShares;

    public ExpenseBuilder()
    {
        _expenseShares = new List<ExpenseShare>(); // Ensuring fresh list for each new expense
    }

    public ExpenseBuilder SetUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public ExpenseBuilder SetFamilyId(Guid? familyId)
    {
        _familyId = familyId;
        return this;
    }

    public ExpenseBuilder SetCategory(Category category)
    {
        _category = category;
        return this;
    }

    public ExpenseBuilder SetAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public ExpenseBuilder SetDate(DateTime date)
    {
        _date = date;
        return this;
    }

    // Add expense shares with validation
    public ExpenseBuilder AddExpenseShares(IEnumerable<(Guid userId, decimal percentage)> shares)
    {
        if (shares.Any() && shares.Sum(s => s.percentage) != 1.0m)
        {
            throw new ArgumentException("Total share percentage must equal 100%.");
        }

        foreach (var share in shares)
        {
            _expenseShares.Add(new ExpenseShare
            {
                UserId = share.userId,
                Percentage = share.percentage
            });
        }

        return this;
    }

    public Expense Build()
    {
        return new Expense(_userId, _familyId, _category, _amount, _date, _expenseShares);
    }
}

public class Expense
{
    public Expense() { }
    
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

    public void AddShares(IEnumerable<ExpenseShare> shares)
    {
        if (shares.Any() && shares.Sum(s => s.Percentage) != 1.0m)
        {
            throw new ArgumentException("Total share percentage must equal 100%.");
        }

        ExpenseShares.AddRange(shares);
    }
}

