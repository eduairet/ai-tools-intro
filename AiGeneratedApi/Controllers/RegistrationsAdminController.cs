using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using AutoMapper;
using EventManagementApi.Models.Dto.EventRegistration;
using EventManagementApi.Shared.Constants;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.RegistrationsAdmin)]
[Authorize]
public class RegistrationsAdminController(IRepositoryEventsRegistrations registrationRepository, IMapper mapper)
    : ControllerBase
{
    // GET: api/v1/events-registrations
    [HttpGet]
    public async Task<IActionResult> GetAllRegistrations()
    {
        var registrations = await registrationRepository.GetAllAsync();
        var response = mapper.Map<IEnumerable<EventRegistrationResponseDto>>(registrations);
        return Ok(response);
    }

    // GET: api/v1/events-registrations/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegistration(string id)
    {
        var registration = await registrationRepository.GetByIdAsync(id);
        if (registration is null)
            return NotFound();

        var response = mapper.Map<EventRegistrationResponseDto>(registration);
        return Ok(response);
    }
}