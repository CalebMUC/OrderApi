using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class OrderProducts
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Order
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [ForeignKey("order")]
        public string OrderID { get; set; }

        public Orders order { get; set; }

        // Foreign key to Product
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [ForeignKey("Product")]
        public string ProductID { get; set; }

        public Products Product { get; set; }


        [Required]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }
    }
}
