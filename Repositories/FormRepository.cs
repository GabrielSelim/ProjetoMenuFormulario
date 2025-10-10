using FormEngineAPI.Models;
using FormEngineAPI.Data;

namespace FormEngineAPI.Repositories;

public interface IFormRepository : IRepository<Form>
{
}

public class FormRepository : Repository<Form>, IFormRepository
{
    public FormRepository(ApplicationDbContext context) : base(context)
    {
    }
}
