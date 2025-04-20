using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Models;


namespace Minimart_Api.Repositories.AddressesRepo
{
    public interface IAddressRepo
    {
        Task<Addresses> GetAddressByIdAsync(int addressId);
        Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId);
        Task AddAddressAsync(AddressDTO address);
        Task EditAddressAsync(EditAddressDTO address);

        Task SaveChangesAsync();
    }
}
