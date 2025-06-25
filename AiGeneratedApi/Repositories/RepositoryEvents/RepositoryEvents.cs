using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryEvents
{
    public class RepositoryEvents : RepositoryBase<Event>, IRepositoryEvents
    {
        public RepositoryEvents(AppDbContext context) : base(context)
        {
        }
        // Add event-specific methods here if needed
    }
}
