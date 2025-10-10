using FormEngineAPI.Models;
using FormEngineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FormEngineAPI.Repositories;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ActivityLog>> GetByEntityAsync(string entity, int entityId);
}

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetByEntityAsync(string entity, int entityId)
    {
        return await _dbSet
            .Include(l => l.User)
            .Where(l => l.Entity == entity && l.EntityId == entityId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}
