using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Features.Categories;
using OrgaNestApi.Infrastructure.Database;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Features.Categories
{
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
            category.Should().NotBeNull();
            category.Name.Should().Be("TestCategory");
        }

        [Fact]
        public async Task CreateCategoryAsync_WithExistingName_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CreateCategoryRequest { Name = "Duplicate" };

            // Create an initial category
            await _service.CreateCategoryAsync(request, CancellationToken.None);

            // Act & Assert
            Func<Task> act = async () => await _service.CreateCategoryAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
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
            category.Should().NotBeNull();
            category.Id.Should().Be(createdCategory.Id);
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
            category.Should().NotBeNull();
            category.Id.Should().Be(createdCategory.Id);
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
            updatedCategory.Name.Should().Be("Updated");
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
            category.Should().BeNull();
        }
    }
}
