using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EventManagementApi.Models.Event;
using EventManagementApi.Repositories.RepositoryEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Linq;
using AutoMapper;
using EventManagementApi.Shared.Constants;
using EventManagementApi.Shared.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IRepositoryEvents _eventRepository;
        private readonly IMapper _mapper;

        public EventsController(IRepositoryEvents eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<EventResponseDto>>(events);
            return Ok(response);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(string id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var response = _mapper.Map<EventResponseDto>(eventItem);
            return Ok(response);
        }

        // POST: api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var eventItem = _mapper.Map<Event>(createEventDto);
            eventItem.Id = Guid.NewGuid();
            eventItem.OwnerId = userId;

            await _eventRepository.AddAsync(eventItem);
            await _eventRepository.SaveChangesAsync();

            var response = _mapper.Map<EventResponseDto>(eventItem);
            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.Id }, response);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] UpdateEventDto updateEventDto)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (eventItem.OwnerId != userId)
                return Forbid(Constants.ApiConstants.ErrorMessages.UnauthorizedAccess);

            _mapper.Map(updateEventDto, eventItem);
            await _eventRepository.SaveChangesAsync();

            var response = _mapper.Map<EventResponseDto>(eventItem);
            return Ok(response);
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (eventItem.OwnerId != userId)
                return Forbid(Constants.ApiConstants.ErrorMessages.UnauthorizedAccess);

            _eventRepository.Remove(eventItem);
            await _eventRepository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/events/{id}/image
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadEventImage(string id, IFormFile file)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (eventItem.OwnerId != userId)
                return Forbid(Constants.ApiConstants.ErrorMessages.UnauthorizedAccess);

            if (!Helpers.File.IsValidImage(file))
                return BadRequest("Invalid file type. Only images are allowed.");

            var uploadPath = Helpers.File.GetTempUploadPath();
            var fileName = await Helpers.File.SaveAsync(file, uploadPath);

            eventItem.ImageUrl = $"/temp/{fileName}";
            await _eventRepository.SaveChangesAsync();

            return Ok(new { imageUrl = eventItem.ImageUrl });
        }
    }
}
