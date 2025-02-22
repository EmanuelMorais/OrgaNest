using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Features.Categories;
using OrgaNestApi.Infrastructure.Database;

namespace UnitTests.Features.Categories;

[TestSubject(typeof(CategoryService))]
public class CategoryServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new CategoryService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateCategoryAsync_CreatesCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "TestCategory" };

        // Act
        var category = await _service.CreateCategoryAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(category);
        Assert.Equal("TestCategory", category.Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithExistingName_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Duplicate" };

        // Create an initial category
        await _service.CreateCategoryAsync(request, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateCategoryAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Category1" };
        var createdCategory = await _service.CreateCategoryAsync(request, CancellationToken.None);

        // Act
        var category = await _service.GetCategoryByIdAsync(createdCategory.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(createdCategory.Id, category!.Id);
    }

    [Fact]
    public async Task GetCategoryByNameAsync_ReturnsCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "UniqueCategory" };
        var createdCategory = await _service.CreateCategoryAsync(request, CancellationToken.None);

        // Act
        var category = await _service.GetCategoryByNameAsync("UniqueCategory", CancellationToken.None);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(createdCategory.Id, category!.Id);
    }

    [Fact]
    public async Task UpdateCategoryAsync_UpdatesCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Original" };
        var createdCategory = await _service.CreateCategoryAsync(request, CancellationToken.None);
        var updateRequest = new UpdateCategoryRequest { Name = "Updated" };

        // Act
        var updatedCategory =
            await _service.UpdateCategoryAsync(createdCategory.Id, updateRequest, CancellationToken.None);

        // Assert
        Assert.Equal("Updated", updatedCategory.Name);
    }

    [Fact]
    public async Task DeleteCategoryAsync_DeletesCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "ToDelete" };
        var createdCategory = await _service.CreateCategoryAsync(request, CancellationToken.None);

        // Act
        await _service.DeleteCategoryAsync(createdCategory.Id, CancellationToken.None);
        var category = await _service.GetCategoryByIdAsync(createdCategory.Id, CancellationToken.None);

        // Assert
        Assert.Null(category);
    }
}