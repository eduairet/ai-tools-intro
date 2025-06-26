using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Models;
using EventManagementApi.Repositories.RepositoryEvents;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using EventManagementApi.Repositories.RepositoryUsers;
using AutoMapper;
using EventManagementApi.Models.Dto.EventRegistration;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.EventsRegistrations)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EventsRegistrationController(
    IRepositoryEvents eventsRepository,
    IRepositoryEventsRegistrations registrationsRepository,
    IRepositoryUsers usersRepository,
    IMapper mapper)
    : ControllerBase
{
    // POST: api/v1/events/{eventId}/register
    [HttpPost]
    public async Task<IActionResult> Register(string eventId)
    {
        if (!Guid.TryParse(eventId, out _))
        {
            return BadRequest(Constants.Api.ErrorMessages.InvalidId);
        }

        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        var eventEntity = await eventsRepository.GetByIdAsync(eventId);
        if (eventEntity is null) return NotFound(Constants.Api.ErrorMessages.EventNotFound);

        if (eventEntity.OwnerId == user!.Id)
            return BadRequest(Constants.Api.ErrorMessages.CannotRegisterForOwnEvent);

        if (registrationsRepository.FindAsync(r => r.EventId == eventId && r.UserId == user.Id).Result.Any())
            return BadRequest(Constants.Api.ErrorMessages.AlreadyRegisteredForEvent);

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid().ToString(),
            EventId = eventEntity.Id,
            UserId = user.Id,
            Event = eventEntity,
            User = user
        };

        await registrationsRepository.AddAsync(registration);
        await registrationsRepository.SaveChangesAsync();

        var response = mapper.Map<EventRegistrationResponseDto>(registration);
        return CreatedAtAction(null, new { id = registration.Id }, response);
    }

    // DELETE: api/v1/events/{eventId}/register
    [HttpDelete]
    public async Task<IActionResult> Unregister(string eventId)
    {
        // Validate eventId is a valid Guid
        if (!Guid.TryParse(eventId, out _))
        {
            return BadRequest(Constants.Api.ErrorMessages.InvalidId);
        }

        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        var registrations =
            await registrationsRepository.FindAsync(r => r.EventId == eventId && r.UserId == user!.Id);

        var registration = registrations.FirstOrDefault();
        if (registration is null) return NotFound(Constants.Api.ErrorMessages.RegistrationNotFound);

        registrationsRepository.Remove(registration);
        await registrationsRepository.SaveChangesAsync();

        return NoContent();
    }
}