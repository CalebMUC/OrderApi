//using Authentication_and_Authorization_Api.Models;
using Microsoft.IdentityModel.Tokens;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using Minimart_Api.Mappings;
using Minimart_Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Minimart_Api.Services
{
    public class MyService:  IMyService
    {
        private readonly IRepository _repository;
        private readonly OrderMapper _orderMapper;

        public MyService(IRepository repository,OrderMapper orderMapper)
        {
            _repository = repository;
            _orderMapper = orderMapper;
        }
        public async Task<IEnumerable<TUser>> GetEntitiesAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<ResponseStatus> CreateOrder(Order order) {

            return await _repository.CreateOrder(order);
        }
        public async Task<ResponseStatus> AddOrder(OrderListDto orderDTO)
        {
            return await _repository.AddOrder(orderDTO);
        }
        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            return await _repository.GetAllCountiesAsync();
        }

        public async Task<IEnumerable<Town>> GetTownsByCountyAsync(int countyId)
        {
            return await _repository.GetTownsByCountyAsync(countyId);
        }

        public async Task<IEnumerable<DeliveryStation>> GetDeliveryStationsByTownAsync(int townId)
        {
            return await _repository.GetDeliveryStationsByTownAsync(townId);
        }

        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            return await _repository.GetAddressByIdAsync(addressId);
        }

        public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId)
        {
            return await _repository.GetAddressesByUserIdAsync(userId);
        }

        public async Task AddAddressAsync(AddressDTO address)
        {
            await _repository.AddAddressAsync(address);
            await _repository.SaveChangesAsync();
        }
        public async Task EditAddressAsync(EditAddressDTO address)
        {
            await _repository.EditAddressAsync(address);
            //await _repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<TUser>> GetAsyncUserName(string UserName)
        {
            return await _repository.GetAsyncUserName(UserName);
        }

        public async Task<Status> AddToCart(string CartItems)
        {
            return await _repository.AddToCart(CartItems); 
        }


        public async Task<Status> DeleteCartItems(CartItemsDTO CartItems)
        {
            return await _repository.DeleteCartItems(CartItems);
        }

        public async Task<Status> SaveItems(SaveItemsDTO saveItems)
        {
            return await _repository.SaveItems(saveItems);
        }

        public async Task<UserRegStatus> UserRegistration(string JsonData)
        {
            return await _repository.UserRegistration(JsonData);
        }

        public async Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname)
        {
            return await _repository.GetDashBoardName(Dashboardname);
        }

        public async Task<IEnumerable<CategoryDTO>> GetDashBoardCategories()
        {
            return await _repository.GetDashBoardCategories();
        }


        public async Task<IEnumerable<CartResults>> GetSubCategory(string Categoryname)
        {
            return await _repository.GetSubCategory(Categoryname);
        }

        public async Task<IEnumerable<CartResults>> GetProductsByCategory(int CategoryID)
        {
            return await _repository.GetProductsByCategory(CategoryID);
        }

        public async Task<UserInfo> GetRefreshToken(string JsonData)
        {
            return await _repository.GetRefreshToken(JsonData);
        }

        public async Task<IEnumerable<TProduct>> LoadProductImages(string productID)
        {
            return await _repository.LoadProductImages(productID);
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int UserID)
        {
            return await _repository.GetCartItems(UserID);
        }

        public async Task<IEnumerable<TProduct>> GetSavedItems()
        {
            return await _repository.GetSavedItems();
        }


        public async Task<IEnumerable<TProduct>> FetchAllProducts()
        {
            return await _repository.FetchAllProducts();
        }

        public async Task<ResponseStatus> AddProducts(AddProducts product)
        {
            return await _repository.AddProducts(product);
        }

        //public async Task<LoginResponse> Login(string JsonData)
        //{
        //    return await _repository.Login(JsonData);
        //}

        public async void SaveRefreshToken(string JsonData)
        {
              _repository.SaveRefreshToken(JsonData);
        }


    }
}
