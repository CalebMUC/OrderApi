//using Authentication_and_Authorization_Api.Models;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using System.Collections;

namespace Minimart_Api.Services
{
    public interface IMyService
    {
        Task<IEnumerable<TUser>> GetEntitiesAsync();

        Task<IEnumerable<TUser>> GetAsyncUserName(string UserName);

        Task<Status> AddToCart(string CartItems);

        Task<Status> DeleteCartItems(CartItemsDTO CartItems);

        Task<Status> SaveItems(SaveItemsDTO saveItems);

        Task<IEnumerable<CartResults>> GetCartItems(int UserID);

        Task<IEnumerable<TProduct>> GetSavedItems();

        Task<IEnumerable<TProduct>> FetchAllProducts();

        Task<ResponseStatus> AddProducts(AddProducts product);

        Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname);
        

        Task<IEnumerable<CartResults>> GetSubCategory(string CategoryName);

        Task<IEnumerable<CategoryDTO>> GetDashBoardCategories();


        Task<IEnumerable<CartResults>> GetProductsByCategory(int CategoryID);

        Task<IEnumerable<County>> GetCountiesAsync();
        Task<IEnumerable<Town>> GetTownsByCountyAsync(int countyId);
        Task<IEnumerable<DeliveryStation>> GetDeliveryStationsByTownAsync(int townId);

        //Task<ResponseStatus> CreateOrder(Order order);

        public Task<ResponseStatus> AddOrder(OrderListDto orderDTO);

        //Task<Order> GetOrderByIdAsync(string OrderID);

        Task<Address> GetAddressByIdAsync(int addressId);
        Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId);
        Task AddAddressAsync(AddressDTO address);
        Task EditAddressAsync(EditAddressDTO address);






        Task <UserInfo> GetRefreshToken(string JsonData);

        Task<IEnumerable<TProduct>> LoadProductImages(string ProductID);

        //Task <LoginResponse> Login(string JsonData);

        public void SaveRefreshToken(string JsonData);

        Task<UserRegStatus> UserRegistration(string JsonData);

        
    }
}
