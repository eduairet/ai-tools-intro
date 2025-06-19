using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventManagementApi.Models.User;
using EventManagementApi.Models.Event;

namespace EventManagementApi.Models.EventRegistration
{
    public class EventRegistration
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [ForeignKey("EventId")]
        public EventManagementApi.Models.Event.Event Event { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public EventManagementApi.Models.User.User User { get; set; } = null!;
    }
} 