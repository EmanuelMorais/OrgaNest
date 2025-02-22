using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Infrastructure.Database;

namespace OrgaNestApi.Features.Shopping;

[ApiController]
[Route("api/shopping-lists")]
public class ShoppingListController : ControllerBase
{
    private readonly IShoppingListService _service;

    public ShoppingListController(IShoppingListService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var lists = await _service.GetShoppingListsByUser(userId);
        return Ok(lists);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var list = await _service.GetShoppingListById(id);
        if (list == null)
            return NotFound();

        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShoppingListDto shoppingList)
    {
        var result = await _service.AddShoppingList(shoppingList);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShoppingListDto shoppingList)
    {
        if (id != shoppingList.Id)
            return BadRequest("ID mismatch");

        await _service.UpdateShoppingList(shoppingList);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteShoppingList(id);
        return NoContent();
    }
}
public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _repository;

    public ShoppingListService(IShoppingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShoppingList>> GetShoppingListsByUser(Guid userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<ShoppingList?> GetShoppingListById(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ShoppingListDto> AddShoppingList(CreateShoppingListDto shoppingListDto)
    {
        var shoppingList = shoppingListDto.ToDomain();

        await _repository.AddAsync(shoppingList);
        
        return shoppingList.ToDto();
    }

    public async Task UpdateShoppingList(UpdateShoppingListDto shoppingListDto)
    {
        var shoppingList = shoppingListDto.ToDomain();
        
        await _repository.UpdateAsync(shoppingList);
    }

    public async Task DeleteShoppingList(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
public class ShoppingListRepository : IShoppingListRepository
{
    private readonly AppDbContext _context;

    public ShoppingListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingList?> GetByIdAsync(Guid id)
    {
        return await _context.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }

    public async Task<List<ShoppingList>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ShoppingLists
            .Where(sl => sl.UserId == userId)
            .Include(sl => sl.Items)
            .ToListAsync();
    }

    public async Task AddAsync(ShoppingList shoppingList)
    {
        await _context.ShoppingLists.AddAsync(shoppingList);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Update(shoppingList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var shoppingList = await _context.ShoppingLists.FindAsync(id);
        if (shoppingList != null)
        {
            _context.ShoppingLists.Remove(shoppingList);
            await _context.SaveChangesAsync();
        }
    }
}
