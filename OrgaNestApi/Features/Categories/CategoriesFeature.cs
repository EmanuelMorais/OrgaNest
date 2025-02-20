using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Infrastructure.Database;
using OrgaNestApi.Infrastructure.Extensions;

namespace OrgaNestApi.Features.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // POST: api/Categories
    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Category name is required.");
        }

        // Check if category already exists via service
        var existingCategory = await _categoryService.GetCategoryByNameAsync(request.Name, cancellationToken);

        if (existingCategory != null)
        {
            return Conflict($"Category '{request.Name}' already exists.");
        }

        // Pass the DTO to service to create the category
        var category = await _categoryService.CreateCategoryAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    // Get Category by Id (optional)
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }
    
    // Get Category by Name
    [HttpGet("search")]
    public async Task<ActionResult<Category>> GetCategoryByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetCategoryByNameAsync(name, cancellationToken);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResult<Category>>> GetAll(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var pagedCategories = await _categoryService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(pagedCategories);
    }  
    
    // Update Category by Id
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Category name is required.");
        }

        var existingCategory = await _categoryService.GetCategoryByIdAsync(id,cancellationToken);
        if (existingCategory == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        var updatedCategory = await _categoryService.UpdateCategoryAsync(id, request, cancellationToken);

        return Ok(updatedCategory);
    }
    
    // DELETE: api/categories/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (existingCategory == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        await _categoryService.DeleteCategoryAsync(id, cancellationToken);

        return NoContent(); // 204 No Content
    }
}


public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    // Now accepts DTO and creates the Category entity internally
    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest dto, CancellationToken cancellationToken)
    {
        // Validate the incoming DTO if necessary
        if (string.IsNullOrEmpty(dto.Name))
        {
            throw new ArgumentException("Category name is required.");
        }

        // Check if category already exists
        var existingCategory = await GetCategoryByNameAsync(dto.Name, cancellationToken);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category '{dto.Name}' already exists.");
        }

        // Create a new Category domain entity from DTO
        var category = new Category
        {
            Name = dto.Name
        };

        // Save to DB
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }

    // Get Category by ID
    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories.FindAsync(categoryId);
    }

    // Get all Categories
    public async Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Categories.AsNoTracking()
            .OrderBy(c => c.Name) // or any sorting criteria
            .GetPagedAsync(pageNumber, pageSize, cancellationToken);
    }
    
    // Get Category by Name
    public async Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }
    
    public async Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        category.Name = request.Name;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category;
    }
    
    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(id, cancellationToken);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        _context.Categories.Remove(category);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
