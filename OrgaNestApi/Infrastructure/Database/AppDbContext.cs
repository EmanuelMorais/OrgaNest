namespace OrgaNestApi.Infrastructure.Database;

using OrgaNestApi.Common.Domain;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Family> Families { get; set; }
    public DbSet<UserFamily> UserFamilies { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseShare> ExpenseShares { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    
    public DbSet<ShoppingItem> ShoppingItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enable Foreign Keys for SQLite
        modelBuilder.UseSqliteForeignKeys();

        // Many-to-Many: User <-> Family (Join Table: UserFamily)
        modelBuilder.Entity<UserFamily>()
            .HasKey(uf => new { uf.UserId, uf.FamilyId });

        modelBuilder.Entity<UserFamily>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.UserFamilies)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete UserFamily entry if User is deleted

        modelBuilder.Entity<UserFamily>()
            .HasOne(uf => uf.Family)
            .WithMany(f => f.UserFamilies)
            .HasForeignKey(uf => uf.FamilyId)
            .OnDelete(DeleteBehavior.Cascade); // Delete UserFamily entry if Family is deleted

        // Many-to-Many: Expense <-> User (Join Table: ExpenseShare)
        modelBuilder.Entity<ExpenseShare>()
            .HasKey(es => new { es.ExpenseId, es.UserId });

        modelBuilder.Entity<ExpenseShare>()
            .HasOne(es => es.Expense)
            .WithMany(e => e.ExpenseShares)
            .HasForeignKey(es => es.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade); // Delete ExpenseShare entry if Expense is deleted

        modelBuilder.Entity<ExpenseShare>()
            .HasOne(es => es.User)
            .WithMany(u => u.ExpenseShares)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete ExpenseShare entry if User is deleted

        // Expense: Many-to-One with User (who created it)
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user if expenses exist

        // Expense: Many-to-One with Family (optional)
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Family)
            .WithMany(f => f.Expenses)
            .HasForeignKey(e => e.FamilyId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of family if expenses exist
        
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Category)
            .WithMany() // If Category does not have navigation back to Expense, use .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of category if expenses exist

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id); // Primary Key
            entity.Property(c => c.Name)
                .IsRequired() // Ensure that the Name is required
                .HasMaxLength(100); // Set a maximum length for the Name
        });      
        
        modelBuilder.Entity<ShoppingList>()
            .HasKey(sl => sl.Id);

        modelBuilder.Entity<ShoppingList>()
            .HasOne(sl => sl.User)
            .WithMany(u => u.ShoppingLists)
            .HasForeignKey(sl => sl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingItem>()
            .HasKey(si => si.Id);

        modelBuilder.Entity<ShoppingItem>()
            .HasOne(si => si.ShoppingList)
            .WithMany(sl => sl.Items)
            .HasForeignKey(si => si.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);        
    }
}

// Extension method to enable foreign keys in SQLite
public static class ModelBuilderExtensions
{
    public static void UseSqliteForeignKeys(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Sqlite:ForeignKeys", true);
    }
}

