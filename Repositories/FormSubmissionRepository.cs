using FormEngineAPI.Models;
using FormEngineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FormEngineAPI.Repositories;

public interface IFormSubmissionRepository : IRepository<FormSubmission>
{
    Task<IEnumerable<FormSubmission>> GetByFormIdAsync(int formId);
    Task<IEnumerable<FormSubmission>> GetByUserIdAsync(int userId);
}

public class FormSubmissionRepository : Repository<FormSubmission>, IFormSubmissionRepository
{
    public FormSubmissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FormSubmission>> GetByFormIdAsync(int formId)
    {
        return await _dbSet
            .Include(s => s.User)
            .Include(s => s.Form)
            .Where(s => s.FormId == formId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FormSubmission>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(s => s.User)
            .Include(s => s.Form)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}
