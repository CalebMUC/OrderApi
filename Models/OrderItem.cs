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
        [Column(TypeName = "varchar(50)")]  // Added explicit type for OrderId
        public string OrderId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [ForeignKey("Product")]
        public string? ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }  // Removed explicit type as int maps directly

        [Required]
        [Column(TypeName = "numeric(18,2)")]  // Changed to PostgreSQL's numeric type
        public decimal Price { get; set; }

        // Navigation Properties
        public virtual Orders? Order { get; set; }
        public virtual Products? Product { get; set; }
    }
}