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
        [Column(TypeName = "nvarchar(50)")]
        public string ProductId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Comment { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        [Column(TypeName = "bit")]
        public bool IsVerifiedBuyer { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool IsVisible { get; set; } = true;

        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string? AdminResponse { get; set; }

        // Navigation Properties
        public virtual Products? Product { get; set; }
        public virtual Users? User { get; set; }
    }
}
