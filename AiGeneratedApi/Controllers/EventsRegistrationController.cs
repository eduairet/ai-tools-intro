using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventManagementApi.Models.EventRegistration;
using EventManagementApi.Repositories.RepositoryEventsRegistrations;
using AutoMapper;
using EventManagementApi.Shared.Constants;
using System.IdentityModel.Tokens.Jwt;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/events-registration")]
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

        // GET: api/eventsregistration
        [HttpGet]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var registrations = await _registrationRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<EventRegistrationResponseDto>>(registrations);
            return Ok(response);
        }

        // GET: api/eventsregistration/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistration(string id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
                return NotFound();

            var response = _mapper.Map<EventRegistrationResponseDto>(registration);
            return Ok(response);
        }

        // POST: api/eventsregistration
        [HttpPost]
        public async Task<IActionResult> CreateRegistration([FromBody] CreateEventRegistrationDto createRegistrationDto)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var registration = _mapper.Map<EventRegistration>(createRegistrationDto);
            registration.Id = Guid.NewGuid();
            registration.UserId = userId;

            await _registrationRepository.AddAsync(registration);
            await _registrationRepository.SaveChangesAsync();

            var response = _mapper.Map<EventRegistrationResponseDto>(registration);
            return CreatedAtAction(nameof(GetRegistration), new { id = registration.Id }, response);
        }

        // DELETE: api/eventsregistration/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistration(string id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
                return NotFound();

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (registration.UserId != userId)
                return Forbid(Constants.ApiConstants.ErrorMessages.UnauthorizedAccess);

            _registrationRepository.Remove(registration);
            await _registrationRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
