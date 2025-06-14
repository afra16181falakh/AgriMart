using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class WishlistItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid WishlistId { get; set; }
        public Guid ProductId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
