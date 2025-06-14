using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class Promotion
    {
        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty; // e.g., "Percentage" or "FixedAmount"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}