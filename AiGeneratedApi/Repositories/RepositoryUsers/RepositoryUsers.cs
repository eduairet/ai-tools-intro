using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryUsers
{
    public class RepositoryUsers : RepositoryBase<User>, IRepositoryUsers
    {
        public RepositoryUsers(AppDbContext context) : base(context)
        {
        }
        // Add user-specific methods here if needed
    }
}
