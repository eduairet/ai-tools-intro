using EventManagementApi.Data;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryBase;

namespace EventManagementApi.Repositories.RepositoryUsers;
public class RepositoryUsers(AppDbContext context) : RepositoryBase<User>(context), IRepositoryUsers
{
    // Add user-specific methods here if needed
}
