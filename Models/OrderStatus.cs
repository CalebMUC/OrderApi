using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class OrderStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Status { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Order { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CreatedBy { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string UpdatedBy { get; set; }

        // No Orders navigation property needed here
    }

}
