using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class DeliveryChargeRules
    {
        [Key]
        public Guid Id { get; set; }

        // This property cannot be null, so we give it a default value.
        public string RuleName { get; set; } = string.Empty;

        // These properties ARE allowed to be null, so we add a '?'
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // These properties cannot be null.
        public decimal ChargeAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}