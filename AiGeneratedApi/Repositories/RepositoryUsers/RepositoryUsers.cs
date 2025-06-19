using EventManagementApi.Data;
using EventManagementApi.Models.User;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryUsers
{
    public class RepositoryUsers : RepositoryBase<User>, IRepositoryUsers
    {
        public RepositoryUsers(ApplicationDbContext context) : base(context)
        {
        }
        // Add user-specific methods here if needed
    }
}
