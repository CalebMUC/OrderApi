using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("Cart")]
        public int? CartId { get; set; }

        [ForeignKey("Product")]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar to varchar
        public string? ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }  // Removed explicit Column type as int is standard

        public bool IsActive { get; set; }
        public bool IsBought { get; set; }

        [Column(TypeName = "timestamp")]  // Changed from datetime to timestamp
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "timestamp")]  // Changed from datetime to timestamp
        public DateTime? UpdatedOn { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Products? Products { get; set; }
    }
}