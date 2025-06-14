using System;
using System.ComponentModel.DataAnnotations;

namespace AgriMartAPI.Models 
{
    public class CMSPage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty; 

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty; 

        [Required]
        public string ContentHtml { get; set; } = string.Empty; 

        [StringLength(255)]
        public string? MetaKeywords { get; set; } 

        [StringLength(500)]
        public string? MetaDescription { get; set; } 

        [Required]
        public bool IsPublished { get; set; }

        public int? StatusId { get; set; } 

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; } 

        public DateTime? ModifiedDate { get; set; } 
    }
}