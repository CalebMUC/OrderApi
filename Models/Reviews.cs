using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.Models
{
    public class Reviews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string ProductId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
        public string? Title { get; set; }

        [Column(TypeName = "text")]  // Changed from nvarchar(max)
        public string? Comment { get; set; }

        [Column(TypeName = "timestamp")]  // Changed from datetime
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public bool IsVerifiedBuyer { get; set; } = false;  // Removed bit annotation

        public bool IsVisible { get; set; } = true;  // Removed bit annotation

        [MaxLength(1000)]
        [Column(TypeName = "varchar(1000)")]  // Changed from nvarchar
        public string? AdminResponse { get; set; }

        // Navigation Properties remain unchanged
        public virtual Products? Product { get; set; }
        public virtual Users? User { get; set; }
    }
}