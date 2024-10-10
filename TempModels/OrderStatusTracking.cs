using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.TempModels
{
    public class OrderStatusTracking
    {
        [Key]
        public int Status { get; set; }
        public string StatusMessage { get; set;}
    }
}
