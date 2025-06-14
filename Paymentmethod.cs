using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class PaymentMethod
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; } 

        [StringLength(255)]
        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        public int? SortOrder { get; set; }
        public string? ConfigurationJson { get; set; }
        public int? StatusId { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(100)]
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}