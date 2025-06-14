using System;
using System.Collections.Generic; // Make sure this is present
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this using statement

namespace AgriMartAPI.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid? BillingAddressId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int AddressId { get; set; } 
        public string Status { get; set; } = string.Empty; 
        public Guid OrderStatusId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string? PaymentTransactionIdExternal { get; set; }
        public string? Notes { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // This property will NOT be saved to the database, but it will be used to receive the order items from the API call.
        [NotMapped]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}