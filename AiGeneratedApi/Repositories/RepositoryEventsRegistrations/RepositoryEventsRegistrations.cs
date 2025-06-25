using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryEventsRegistrations
{
    public class RepositoryEventsRegistrations : RepositoryBase<EventRegistration>, IRepositoryEventsRegistrations
    {
        public RepositoryEventsRegistrations(AppDbContext context) : base(context)
        {
        }
        // Add registration-specific methods here if needed
    }
}
