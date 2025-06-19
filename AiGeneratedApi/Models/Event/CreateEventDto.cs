using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagementApi.Models.Event
{
    public class CreateEventDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }
} 