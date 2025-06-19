using AutoMapper;
using EventManagementApi.Models.User;
using EventManagementApi.Models.Event;
using EventManagementApi.Models.EventRegistration;

namespace EventManagementApi.Config
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // User mappings
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Will be set manually

            CreateMap<User, UserResponseDto>();

            // Event mappings
            CreateMap<CreateEventDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore()); // Will be set from JWT

            CreateMap<UpdateEventDto, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore()); // Will be set from JWT

            CreateMap<Event, EventResponseDto>();

            // EventRegistration mappings
            CreateMap<CreateEventRegistrationDto, EventRegistration>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // Will be set from JWT

            CreateMap<EventRegistration, EventRegistrationResponseDto>();
        }
    }
}
