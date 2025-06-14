using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }  
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;  
        public string LastName { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        
        // These properties are allowed to be null
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
