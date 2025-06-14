// In: Models/AppSetting.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models // Ensure this namespace is correct
{
    public class AppSetting
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? SettingKey { get; set; } // Matches UNIQUE constraint in DB

        [Required]
        public string? SettingValue { get; set; } // NVARCHAR(MAX)

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        public bool IsEditableByAdmin { get; set; }

        public int? StatusId { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}