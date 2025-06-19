using EventManagementApi.Data;
using EventManagementApi.Models.EventRegistration;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryEventsRegistrations
{
    public class RepositoryEventsRegistrations : RepositoryBase<EventRegistration>, IRepositoryEventsRegistrations
    {
        public RepositoryEventsRegistrations(ApplicationDbContext context) : base(context)
        {
        }
        // Add registration-specific methods here if needed
    }
}
