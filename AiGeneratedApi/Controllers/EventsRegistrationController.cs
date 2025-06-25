using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using AutoMapper;
using EventManagementApi.Models.Dto.EventRegistration;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsRegistrationController : ControllerBase
{
    private readonly IRepositoryEventsRegistrations _registrationRepository;
    private readonly IMapper _mapper;

    public EventsRegistrationController(IRepositoryEventsRegistrations registrationRepository, IMapper mapper)
    {
        _registrationRepository = registrationRepository;
        _mapper = mapper;
    }

    // POST: api/events/{eventId}/register
    [HttpPost("{eventId}/register")]
    public async Task<IActionResult> Register(string eventId)
    {
        var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid().ToString(),
            EventId = Guid.Parse(eventId).ToString(),
            UserId = userId
        };

        await _registrationRepository.AddAsync(registration);
        await _registrationRepository.SaveChangesAsync();

        var response = _mapper.Map<EventRegistrationResponseDto>(registration);
        return CreatedAtAction(null, new { id = registration.Id }, response);
    }

    // DELETE: api/events/{eventId}/register
    [HttpDelete("{eventId}/register")]
    public async Task<IActionResult> Unregister(string eventId)
    {
        var userId = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var eventGuid = Guid.Parse(eventId);
        var registrations =
            await _registrationRepository.FindAsync(r => r.EventId == eventGuid.ToString() && r.UserId == userId);
        var registration = registrations.FirstOrDefault();
        if (registration is null)
            return NotFound();

        _registrationRepository.Remove(registration);
        await _registrationRepository.SaveChangesAsync();

        return NoContent();
    }
}