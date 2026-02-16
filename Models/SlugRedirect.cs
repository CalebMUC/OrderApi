using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.Models
{
    /// <summary>
    /// Tracks old product slugs for 301 redirects when product names change
    /// </summary>
    public class SlugRedirect
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(300)]
        public string OldSlug { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string NewSlug { get; set; } = string.Empty;

        [Required]
        public Guid ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}