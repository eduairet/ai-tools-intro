using EventManagementApi.Models.Event;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryEvents
{
    public interface IRepositoryEvents : IRepositoryBase<Event>
    {
        // Add event-specific methods here if needed
    }
}
