using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Repositories.RepositoryEventsRegistrations;

public class RepositoryEventsRegistrations(AppDbContext context)
    : RepositoryBase<EventRegistration>(context), IRepositoryEventsRegistrations
{
    public override async Task<EventRegistration?> GetByIdAsync(object id)
    {
        return await Context.EventRegistrations
            .Include(er => er.Event)
            .Include(er => er.User)
            .FirstOrDefaultAsync(er => er.Id == (string)id);
    }

    public override async Task<IEnumerable<EventRegistration>> GetAllAsync()
    {
        return await Context.EventRegistrations
            .Include(er => er.Event)
            .Include(er => er.User)
            .ToListAsync();
    }
}