using System.Security.Claims;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryUsers;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Shared.Helpers;

public static class UserHelper
{
    public static async Task<(bool Success, User? User, IActionResult? ErrorResult)> GetAndValidateCurrentUserAsync(
        ClaimsPrincipal userClaims,
        IRepositoryUsers usersRepository)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return (false, null, new UnauthorizedResult());

        var user = await usersRepository.GetByIdAsync(userId);
        if (user is null)
            return (false, null, new NotFoundObjectResult(Constants.Constants.Api.ErrorMessages.UserNotFound));

        return (true, user, null);
    }
}
