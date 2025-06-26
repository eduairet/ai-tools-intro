using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using EventManagementApi.Repositories.RepositoryUsers;
using AutoMapper;
using EventManagementApi.Models.Dto.EventRegistration;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.RegistrationsAdmin)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RegistrationsAdminController(
    IRepositoryEventsRegistrations registrationsRepository,
    IRepositoryUsers usersRepository,
    IMapper mapper)
    : ControllerBase
{
    // GET: api/v1/events-registrations
    [HttpGet]
    public async Task<IActionResult> GetAllRegistrations()
    {
        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        var registrations = (await registrationsRepository.GetAllAsync()).Where(r => r.Event.OwnerId == user!.Id);

        var response = mapper.Map<IEnumerable<EventRegistrationResponseDto>>(registrations);
        return Ok(response);
    }

    // GET: api/v1/events-registrations/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegistration(string id)
    {
        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        var registration = await registrationsRepository.GetByIdAsync(id);
        if (registration is null || registration.UserId != user?.Id) return NotFound();

        var response = mapper.Map<EventRegistrationResponseDto>(registration);
        return Ok(response);
    }
}