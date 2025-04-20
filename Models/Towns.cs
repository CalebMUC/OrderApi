using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Towns
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string TownName { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        [Required]
        public int CountyId { get; set; }

        [ForeignKey("CountyId")]
        public virtual Counties County { get; set; }

        public virtual ICollection<DeliveryStations> DeliveryStations { get; set; }
    }
}
