namespace Minimart_Api.DTOS
{
    public class GetAddressDTO
    {
        public int AddressID { get; set; }     // The unique ID for the address
        public int UserID { get; set; }        // The user's ID to which this address belongs
        public string Name { get; set; }       // Name associated with the address
        public string PhoneNumber { get; set; } // Phone number for contact or delivery
        public string PostalAddress { get; set; } // Postal address information
        public int CountyId { get; set; }      // ID of the county
        public int TownId { get; set; }        // ID of the town

        public string ExtraInformation { get; set; }
        public int isDefault { get; set; }
        public string PostalCode { get; set; } // Postal code for the address
        public string County { get; set; }
        public string Town { get; set; }
    }
}
