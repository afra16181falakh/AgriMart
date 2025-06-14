using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? NotificationType { get; set; } // e.g., "OrderUpdate", "Promotion"
        public string? LinkUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}