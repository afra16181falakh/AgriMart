using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}