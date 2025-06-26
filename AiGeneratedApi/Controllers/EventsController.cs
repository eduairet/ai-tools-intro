using Microsoft.AspNetCore.Mvc;
using EventManagementApi.Models;
using EventManagementApi.Models.Dto.Event;
using EventManagementApi.Repositories.RepositoryEvents;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace EventManagementApi.Controllers;

[ApiController]
[Route(Constants.Api.Routes.Events)]
[Authorize]
public class EventsController(IRepositoryEvents eventRepository, IMapper mapper) : ControllerBase
{
    // GET: api/v1/events
    [HttpGet]
    [AllowAnonymous] // Allow anonymous access for public event listings
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await eventRepository.GetAllAsync();
        var response = mapper.Map<IEnumerable<EventResponseDto>>(events);
        return Ok(response);
    }

    // GET: api/v1/events/{id}
    [HttpGet("{id}")]
    [AllowAnonymous] // Allow anonymous access for public event details
    public async Task<IActionResult> GetEvent(string id)
    {
        var eventItem = await eventRepository.GetByIdAsync(id);

        if (eventItem is null) return NotFound();

        var response = mapper.Map<EventResponseDto>(eventItem);
        return Ok(response);
    }

    // POST: api/v1/events
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var eventItem = mapper.Map<Event>(createEventDto);
        eventItem.Id = Guid.NewGuid().ToString();
        eventItem.OwnerId = userId;

        await eventRepository.AddAsync(eventItem);
        await eventRepository.SaveChangesAsync();

        var response = mapper.Map<EventResponseDto>(eventItem);
        return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, response);
    }

    // PUT: api/v1/events/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto updateEventDto)
    {
        var eventItem = await eventRepository.GetByIdAsync(id);

        if (eventItem is null) return NotFound();

        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (eventItem.OwnerId != userId)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        mapper.Map(updateEventDto, eventItem);
        await eventRepository.SaveChangesAsync();

        var response = mapper.Map<EventResponseDto>(eventItem);
        return Ok(response);
    }

    // DELETE: api/v1/events/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(string id)
    {
        var eventItem = await eventRepository.GetByIdAsync(id);

        if (eventItem is null) return NotFound();

        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (eventItem.OwnerId != userId)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        eventRepository.Remove(eventItem);
        await eventRepository.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/v1/events/{id}/image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadEventImage(string id, IFormFile file)
    {
        var eventItem = await eventRepository.GetByIdAsync(id);
        if (eventItem is null)
            return NotFound();

        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (eventItem.OwnerId != userId)
            return Forbid(Constants.Api.ErrorMessages.UnauthorizedAccess);

        if (!Helpers.File.IsValidImage(file))
            return BadRequest(Constants.Api.ErrorMessages.OnlyImagesAllowed);

        var uploadPath = Helpers.File.GetTempUploadPath();
        var fileName = await Helpers.File.SaveAsync(file, uploadPath);

        eventItem.ImageUrl = $"/{Constants.Api.FileUpload.TempFolder}/{fileName}";
        await eventRepository.SaveChangesAsync();

        return Ok(new { imageUrl = eventItem.ImageUrl });
    }
}