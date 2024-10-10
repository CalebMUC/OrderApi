using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class Address
    {
        public int AddressID { get; set; }
        public int UserID { get; set; } // Foreign key
        public string Name { get; set; }
        public string Phonenumber { get; set; }
        public string PostalAddress { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
        public string PostalCode { get; set; }
        public string ExtraInformation { get; set; }
        public int isDefault { get; set; }

        // Navigation property
        public TUser TUser { get; set; }
    }
}
