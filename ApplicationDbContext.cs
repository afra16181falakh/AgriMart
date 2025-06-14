using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class DeliveryChargeRule
    {
        [Key]
        public Guid Id { get; set; }

        // THE FIX IS HERE: We give it a default starting value.
        public string RuleName { get; set; } = string.Empty;

        // THE FIX IS HERE: We allow this one to be null.
        public string? Description { get; set; }

        public decimal ChargeAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? SortOrder { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}