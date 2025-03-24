using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.TempModels
{
    public class OrderStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string Status { get; set;}
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
