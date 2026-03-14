using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.Models
{
    /// <summary>
    /// Server-side cart storage for guest users (7-day expiration)
    /// </summary>
    public class GuestCart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuestCartId { get; set; }

        /// <summary>
        /// Anonymous identifier for the guest (device fingerprint, session ID, UUID)
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string GuestId { get; set; } = string.Empty;

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 9999)]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "timestamp with time zone")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Auto-delete carts after 7 days of inactivity
        /// </summary>
        [Column(TypeName = "timestamp with time zone")]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
