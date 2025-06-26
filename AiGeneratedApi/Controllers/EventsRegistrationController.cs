using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using AutoMapper;
using EventManagementApi.Models.Dto.EventRegistration;
using EventManagementApi.Shared.Constants;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.EventsRegistration)]
[Authorize]
public class EventsRegistrationController(IRepositoryEventsRegistrations registrationRepository, IMapper mapper)
    : ControllerBase
{
    // POST: api/v1/events/{eventId}/register
    [HttpPost]
    public async Task<IActionResult> Register(string eventId)
    {
        var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid().ToString(),
            EventId = Guid.Parse(eventId).ToString(),
            UserId = userId
        };

        await registrationRepository.AddAsync(registration);
        await registrationRepository.SaveChangesAsync();

        var response = mapper.Map<EventRegistrationResponseDto>(registration);
        return CreatedAtAction(null, new { id = registration.Id }, response);
    }

    // DELETE: api/v1/events/{eventId}/register
    [HttpDelete]
    public async Task<IActionResult> Unregister(string eventId)
    {
        var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var eventGuid = Guid.Parse(eventId);
        var registrations =
            await registrationRepository.FindAsync(r => r.EventId == eventGuid.ToString() && r.UserId == userId);
        var registration = registrations.FirstOrDefault();

        if (registration is null) return NotFound();

        registrationRepository.Remove(registration);
        await registrationRepository.SaveChangesAsync();

        return NoContent();
    }
}