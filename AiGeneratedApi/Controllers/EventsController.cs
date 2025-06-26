using Microsoft.AspNetCore.Mvc;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.Event;
using EventManagementApi.Repositories.RepositoryEvents;
using EventManagementApi.Repositories.RepositoryUsers;
using Microsoft.AspNetCore.Authorization;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.Events)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EventsController(IRepositoryEvents eventsRepository, IRepositoryUsers usersRepository, IMapper mapper)
    : ControllerBase
{
    // GET: api/v1/events
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await eventsRepository.GetAllAsync();
        var response = mapper.Map<IEnumerable<EventResponseDto>>(events);
        return Ok(response);
    }

    // GET: api/v1/events/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvent(string id)
    {
        var eventItem = await eventsRepository.GetByIdAsync(id);

        if (eventItem is null) return NotFound(Constants.Api.ErrorMessages.EventNotFound);

        var response = mapper.Map<EventResponseDto>(eventItem);
        return Ok(response);
    }

    // POST: api/v1/events
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        var eventItem = mapper.Map<Event>(createEventDto);
        eventItem.Id = Guid.NewGuid().ToString();
        eventItem.OwnerId = user!.Id;
        eventItem.Owner = user;

        await eventsRepository.AddAsync(eventItem);
        await eventsRepository.SaveChangesAsync();

        var response = mapper.Map<EventResponseDto>(eventItem);

        return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, response);
    }

    // PUT: api/v1/events/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto updateEventDto)
    {
        var eventItem = await eventsRepository.GetByIdAsync(id);
        if (eventItem is null) return NotFound();

        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        if (eventItem.OwnerId != user!.Id)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        mapper.Map(updateEventDto, eventItem);
        await eventsRepository.SaveChangesAsync();

        var response = mapper.Map<EventResponseDto>(eventItem);
        return Ok(response);
    }

    // DELETE: api/v1/events/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(string id)
    {
        var eventItem = await eventsRepository.GetByIdAsync(id);
        if (eventItem is null) return NotFound();

        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        if (eventItem.OwnerId != user!.Id)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        eventsRepository.Remove(eventItem);
        await eventsRepository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/v1/events/{id}/image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadEventImage(string id, IFormFile file)
    {
        var eventItem = await eventsRepository.GetByIdAsync(id);
        if (eventItem is null) return NotFound();

        var (success, user, errorResult) = await UserHelper.GetAndValidateCurrentUserAsync(User, usersRepository);
        if (!success) return errorResult!;

        if (eventItem.OwnerId != user!.Id)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        if (!Helpers.File.IsValidImage(file))
            return BadRequest(Constants.Api.ErrorMessages.OnlyImagesAllowed);

        var uploadPath = Helpers.File.GetTempUploadPath();
        var fileName = await Helpers.File.SaveAsync(file, uploadPath);

        eventItem.ImageUrl = $"/{Constants.Api.FileUpload.TempFolder}/{fileName}";
        await eventsRepository.SaveChangesAsync();

        return Ok(new { imageUrl = eventItem.ImageUrl });
    }
}