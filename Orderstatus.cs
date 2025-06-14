using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class OrderStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public int? SortOrder { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Name
        {
            get => StatusName;
            set => StatusName = value;
        }
    }
}