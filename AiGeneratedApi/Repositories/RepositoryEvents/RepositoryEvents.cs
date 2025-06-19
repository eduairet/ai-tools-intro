using EventManagementApi.Data;
using EventManagementApi.Models.Event;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryEvents
{
    public class RepositoryEvents : RepositoryBase<Event>, IRepositoryEvents
    {
        public RepositoryEvents(ApplicationDbContext context) : base(context)
        {
        }
        // Add event-specific methods here if needed
    }
}
