using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Cart
    {
        //public Cart()
        //{
        //    CartItems = new HashSet<CartItem>();
        //}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        [Required]
        public int? UserId { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? CartName { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation property to user
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        // Navigation property for cart items
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
