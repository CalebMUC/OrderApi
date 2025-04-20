using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Models;
using Minimart_Api.Repositories.AddressesRepo;

namespace Minimart_Api.Services.Address
{

    public class AddressService : IAddress
    {
        private readonly IAddressRepo _addressRepo;

        public AddressService(IAddressRepo addressRepo)
        {
            _addressRepo = addressRepo;
        }
        public async Task<Addresses> GetAddressByIdAsync(int addressId)
        {
            return await _addressRepo.GetAddressByIdAsync(addressId);
        }

        public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId)
        {
            return await _addressRepo.GetAddressesByUserIdAsync(userId);
        }

        public async Task AddAddressAsync(AddressDTO address)
        {
            await _addressRepo.AddAddressAsync(address);
            await _addressRepo.SaveChangesAsync();
        }
        public async Task EditAddressAsync(EditAddressDTO address)
        {
            await _addressRepo.EditAddressAsync(address);
            //await _repository.SaveChangesAsync();
        }
    }
}
