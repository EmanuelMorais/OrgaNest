using Microsoft.AspNetCore.Mvc;
using Moq;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Categories;

namespace UnitTests.Features.Categories;

public class CategoriesControllerTests
{
    private readonly CategoriesController _controller;
    private readonly Mock<ICategoryService> _mockService;

    public CategoriesControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoriesController(_mockService.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "" };

        // Act
        var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Category name is required.", badRequest.Value);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithExistingCategory_ReturnsConflict()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "TestCategory" };
        _mockService
            .Setup(s => s.GetCategoryByNameAsync("TestCategory", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Id = Guid.NewGuid(), Name = "TestCategory" });

        // Act
        var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

        // Assert
        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Contains("Category 'TestCategory' already exists", conflict.Value.ToString());
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "NewCategory" };
        var newCategory = new Category { Id = Guid.NewGuid(), Name = "NewCategory" };

        _mockService
            .Setup(s => s.GetCategoryByNameAsync("NewCategory", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        _mockService
            .Setup(s => s.CreateCategoryAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCategory);

        // Act
        var result = await _controller.CreateCategoryAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetCategoryById), createdResult.ActionName);
        Assert.Equal(newCategory, createdResult.Value);
    }

    [Fact]
    public async Task GetCategoryById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _controller.GetCategoryById(categoryId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCategoryById_WhenFound_ReturnsCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "TestCategory" };
        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategoryById(categoryId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Category>>(result);
        Assert.Equal(category, actionResult.Value);
    }

    [Fact]
    public async Task GetCategoryByName_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetCategoryByNameAsync("NonExisting", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _controller.GetCategoryByName("NonExisting", CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCategoryByName_WhenFound_ReturnsCategory()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "FoundCategory" };
        _mockService
            .Setup(s => s.GetCategoryByNameAsync("FoundCategory", It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategoryByName("FoundCategory", CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Category>>(result);
        Assert.Equal(category, actionResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<Category>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalRecords = 50,
            Data = new List<Category>
            {
                new() { Id = Guid.NewGuid(), Name = "Category1" },
                new() { Id = Guid.NewGuid(), Name = "Category2" }
            }
        };
        _mockService
            .Setup(s => s.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetAll(1, 10, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(pagedResult, okResult.Value);
    }

    [Fact]
    public async Task UpdateCategory_WhenNameIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest { Name = "" };

        // Act
        var result = await _controller.UpdateCategory(categoryId, request, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Category name is required.", badRequest.Value);
    }

    [Fact]
    public async Task UpdateCategory_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest { Name = "UpdatedName" };
        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _controller.UpdateCategory(categoryId, request, CancellationToken.None);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Category with ID {categoryId} not found.", notFound.Value);
    }

    [Fact]
    public async Task UpdateCategory_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category { Id = categoryId, Name = "OldName" };
        var request = new UpdateCategoryRequest { Name = "NewName" };
        var updatedCategory = new Category { Id = categoryId, Name = "NewName" };

        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);
        _mockService
            .Setup(s => s.UpdateCategoryAsync(categoryId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _controller.UpdateCategory(categoryId, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedCategory, okResult.Value);
    }

    [Fact]
    public async Task DeleteCategory_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _controller.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Category with ID {categoryId} not found.", notFound.Value);
    }

    [Fact]
    public async Task DeleteCategory_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category { Id = categoryId, Name = "ToDelete" };
        _mockService
            .Setup(s => s.GetCategoryByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _controller.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}