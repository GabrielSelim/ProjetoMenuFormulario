using FormEngineAPI.Models;
using FormEngineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FormEngineAPI.Repositories;

public interface IFormRepository : IRepository<Form>
{
    Task<IEnumerable<Form>> GetVersionsByOriginalIdAsync(int originalFormId);
    Task<Form?> GetLatestVersionAsync(int originalFormId);
    Task<Form?> GetSpecificVersionAsync(int originalFormId, string version);
    Task<string> GetNextVersionAsync(int originalFormId);
    Task MarkAllVersionsAsNotLatestAsync(int originalFormId);
}

public class FormRepository : Repository<Form>, IFormRepository
{
    public FormRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Form>> GetVersionsByOriginalIdAsync(int originalFormId)
    {
        return await _context.Set<Form>()
            .Where(f => f.OriginalFormId == originalFormId || f.Id == originalFormId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<Form?> GetLatestVersionAsync(int originalFormId)
    {
        return await _context.Set<Form>()
            .Where(f => (f.OriginalFormId == originalFormId || f.Id == originalFormId) && f.IsLatest)
            .FirstOrDefaultAsync();
    }

    public async Task<Form?> GetSpecificVersionAsync(int originalFormId, string version)
    {
        return await _context.Set<Form>()
            .Where(f => (f.OriginalFormId == originalFormId || f.Id == originalFormId) && f.Version == version)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetNextVersionAsync(int originalFormId)
    {
        var versions = await _context.Set<Form>()
            .Where(f => f.OriginalFormId == originalFormId || f.Id == originalFormId)
            .Select(f => f.Version)
            .ToListAsync();

        if (!versions.Any())
            return "1.0";

        var maxVersion = versions
            .Select(v => 
            {
                if (decimal.TryParse(v, out var parsed))
                    return parsed;
                return 1.0m;
            })
            .Max();

        return (maxVersion + 1.0m).ToString("0.0");
    }

    public async Task MarkAllVersionsAsNotLatestAsync(int originalFormId)
    {
        var forms = await _context.Set<Form>()
            .Where(f => (f.OriginalFormId == originalFormId || f.Id == originalFormId) && f.IsLatest)
            .ToListAsync();

        foreach (var form in forms)
        {
            form.IsLatest = false;
        }

        await _context.SaveChangesAsync();
    }
}
