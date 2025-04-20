
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class SavedItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ProductId { get; set; }

        [Required]
        public DateTime SavedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public int Quantity { get; set; }

        public bool IsActive{get; set;}

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }

}
