using FormEngineAPI.Models;
using FormEngineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FormEngineAPI.Repositories;

public interface IMenuRepository : IRepository<Menu>
{
    Task<IEnumerable<Menu>> GetMenusWithChildrenAsync();
}

public class MenuRepository : Repository<Menu>, IMenuRepository
{
    public MenuRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Menu>> GetMenusWithChildrenAsync()
    {
        return await _dbSet
            .Include(m => m.Children)
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.Order)
            .ToListAsync();
    }
}
