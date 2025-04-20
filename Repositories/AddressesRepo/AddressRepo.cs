using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.AddressesRepo
{
    public class AddressRepo
    {

        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<Categories> _logger;

        public AddressRepo(MinimartDBContext dBContext, ILogger<Categories> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }
        public async Task<Addresses> GetAddressByIdAsync(int addressId)
        {
            return await _dbContext.Addresses.FindAsync(addressId);
        }

        public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _dbContext.Addresses
                                             .Where(a => a.UserID == userId)
                                             .ToListAsync();

            // List to hold the addresses with the correct County and Town IDs
            var addressDTOs = new List<GetAddressDTO>();

            foreach (var address in addresses)
            {
                // Fetch the County ID based on the county name
                var county = await _dbContext.Counties
                                             .FirstOrDefaultAsync(c => c.CountyName == address.County);
                // Fetch the Town ID based on the town name
                var town = await _dbContext.Towns
                                           .FirstOrDefaultAsync(t => t.TownName == address.Town);

                // Check if both county and town were found
                if (county != null && town != null)

                {
                    // Create an AddressDTO or update address with CountyID and TownID
                    addressDTOs.Add(new GetAddressDTO
                    {
                        AddressID = address.AddressID,
                        UserID = address.UserID,
                        Name = address.Name,
                        PhoneNumber = address.Phonenumber,
                        PostalAddress = address.PostalAddress,
                        CountyId = county.CountyId, // Use CountyID
                        TownId = town.TownId,       // Use TownID
                        PostalCode = address.PostalCode,
                        County = county.CountyName,
                        Town = county.CountyName,
                        ExtraInformation = address.ExtraInformation,
                        isDefault = address.isDefault,
                    });
                }
            }

            return addressDTOs;
        }


        public async Task AddAddressAsync(AddressDTO address)
        {
            var NewAddress = new Addresses
            {
                UserID = address.UserID,
                Name = address.Name,
                Phonenumber = address.Phonenumber,
                PostalAddress = address.PostalAddress,
                County = address.County,
                Town = address.Town,
                PostalCode = address.PostalCode,
                ExtraInformation = address.ExtraInformation,
                isDefault = address.isDefault,
            };
            await _dbContext.Addresses.AddAsync(NewAddress);
        }
        public async Task EditAddressAsync(EditAddressDTO address)
        {
            // Find the existing address by ID
            var existingAddress = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.AddressID == address.AddressID);

            if (address.isDefault)
            {
                // Find and reset any previous default address for the user
                var defaultAddress = await _dbContext.Addresses
                    .FirstOrDefaultAsync(a => a.UserID == address.UserID && a.isDefault == true && a.AddressID != address.AddressID);

                if (defaultAddress != null)
                {
                    defaultAddress.isDefault = true;
                    _dbContext.Addresses.Update(defaultAddress);
                }
            }

            // Update the existing address fields with new data
            //existingAddress.UserID = address.UserID;
            existingAddress.Name = address.Name;
            existingAddress.Phonenumber = address.Phonenumber;
            existingAddress.PostalAddress = address.PostalAddress;
            existingAddress.County = address.County;
            existingAddress.Town = address.Town;
            existingAddress.PostalCode = address.PostalCode;
            existingAddress.ExtraInformation = address.ExtraInformation;
            existingAddress.isDefault = address.isDefault;

            _dbContext.Addresses.Update(existingAddress);

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();
        }
    }
}
