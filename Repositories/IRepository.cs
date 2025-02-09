using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using System.Collections;

namespace Minimart_Api.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<TUser>> GetAllAsync();

        Task<IEnumerable<TUser>> GetAsyncUserName(string username);

        Task<Status> AddToCart(string CartItems);

        Task<Status> DeleteCartItems(CartItemsDTO CartItems);

        Task<Status> SaveItems(SaveItemsDTO saveItems);

        Task<UserRegStatus> UserRegistration(string CartItems);
        
        Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname);

        Task<IEnumerable<CategoryDTO>> GetDashBoardCategories();

        Task<IEnumerable<CartResults>> GetSubCategory(string Categoryname);

        Task<IEnumerable<CartResults>> GetProductsByCategory(int CategoryID);

        Task <UserInfo> GetRefreshToken(string Categoryname);

        Task<IEnumerable<County>> GetAllCountiesAsync();
        Task<IEnumerable<Town>> GetTownsByCountyAsync(int countyId);
        Task<IEnumerable<DeliveryStation>> GetDeliveryStationsByTownAsync(int townId);

        Task<IEnumerable<TProduct>> LoadProductImages(string ProductID);

        Task<IEnumerable<CartResults>> GetCartItems(int UserID);

        Task<IEnumerable<TProduct>> GetSavedItems();


        Task<ResponseStatus> CreateOrder(Order order);

        public Task<ResponseStatus> AddOrder(OrderListDto orderDTO);

        Task<Address> GetAddressByIdAsync(int addressId);
        Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId);
        Task AddAddressAsync(AddressDTO address);
        Task EditAddressAsync(EditAddressDTO address);
        Task SaveChangesAsync();

        Task<IEnumerable<TProduct>> FetchAllProducts();

        Task<ResponseStatus> AddProducts(AddProducts product);


        //Task <LoginResponse> Login(string JsonData);

        public Task SaveRefreshToken(string JsonData);
    }
}
