using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Categories;

public interface ICategoryService
{
    Task<Category> CreateCategoryAsync(CreateCategoryRequest dto, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken);
    Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);
}