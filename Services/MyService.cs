using Authentication_and_Authorization_Api.Models;
using Microsoft.IdentityModel.Tokens;
using Minimart_Api.DTOS;
using Minimart_Api.Models;
using Minimart_Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Minimart_Api.Services
{
    public class MyService:  IMyService
    {
        private readonly IRepository _repository;

        public MyService(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<TUser>> GetEntitiesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<TUser>> GetAsyncUserName(string UserName)
        {
            return await _repository.GetAsyncUserName(UserName);
        }

        public async Task<Status> AddToCart(string CartItems)
        {
            return await _repository.AddToCart(CartItems); 
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

        public async Task<IEnumerable<CartResults>> GetProductsByCategory(string CategoryID)
        {
            return await _repository.GetProductsByCategory(CategoryID);
        }

        public async Task<UserInfo> GetRefreshToken(string JsonData)
        {
            return await _repository.GetRefreshToken(JsonData);
        }

        public async Task<IEnumerable<TProduct>> LoadProductImages(int productID)
        {
            return await _repository.LoadProductImages(productID);
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int UserID)
        {
            return await _repository.GetCartItems(UserID);
        }


        public async Task<IEnumerable<TProduct>> FetchAllProducts()
        {
            return await _repository.FetchAllProducts();
        }

        public async Task<ResponseStatus> AddProducts(AddProducts product)
        {
            return await _repository.AddProducts(product);
        }

        public async Task<UserInfo> Login(string JsonData)
        {
            return await _repository.Login(JsonData);
        }

        public async void SaveRefreshToken(string JsonData)
        {
              _repository.SaveRefreshToken(JsonData);
        }


    }
}
