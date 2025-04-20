using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")]
        public string OrderId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [ForeignKey("Product")]
        public string? ProductId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation Properties
        public virtual Orders? Order { get; set; }
        public virtual Products? Product { get; set; }
    }
}
