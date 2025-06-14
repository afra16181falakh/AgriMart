using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class CartItem
    {
        [Key]
        public Guid UserId { get; set; } // Keep this one
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        // REMOVE THE LINE BELOW: public Guid UserId { get; set; }
    }
}