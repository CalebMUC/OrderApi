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

        //[ForeignKey("Product")]
        [Column(TypeName = "varchar(50)")]  
        public string? ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } 

        public bool IsActive { get; set; }
        public bool IsBought { get; set; }

        [Column(TypeName = "timestamp")]  
        public DateTime? CreatedOn { get; set; }

        [Column(TypeName = "timestamp")]  
        public DateTime? UpdatedOn { get; set; }

        public virtual Cart? Cart { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products? Products { get; set; }
    }
}