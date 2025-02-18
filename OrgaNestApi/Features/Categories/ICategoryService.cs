using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Categories;

public interface ICategoryService
{
    Task<Category> CreateCategoryAsync(CreateCategoryRequest dto);
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<Category?> GetCategoryByNameAsync(string name);
    Task<List<Category>> GetAllAsync();
    Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
    Task DeleteCategoryAsync(Guid id);
}
