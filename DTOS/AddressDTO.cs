namespace Minimart_Api.DTOS
{
    public class AddressDTO
    {
        
        public int UserID { get; set; } // Foreign key
        public string Name { get; set; }
        public string Phonenumber { get; set; }
        public string PostalAddress { get; set; }
        public string County { get; set; }
        public string Town { get; set; }
       public string ExtraInformation { get; set; }
        public int isDefault { get; set; }
        public string PostalCode { get; set; }
    }
}
