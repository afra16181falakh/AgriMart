// In AgriMartAPI.Models/Category.cs
using System; // Make sure this is present for Guid type

namespace AgriMartAPI.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } // ADDED 'required' KEYWORD HERE
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int? SortOrder { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}