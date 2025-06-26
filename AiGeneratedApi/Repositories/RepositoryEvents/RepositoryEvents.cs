using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Repositories.RepositoryEvents;

public class RepositoryEvents(AppDbContext context) : RepositoryBase<Event>(context), IRepositoryEvents
{
    public override async Task<Event?> GetByIdAsync(object id)
    {
        return await Context.Events
            .Include(e => e.Owner)
            .FirstOrDefaultAsync(e => e.Id == id.ToString());
    }

    public override async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await Context.Events
            .Include(e => e.Owner)
            .ToListAsync();
    }
}