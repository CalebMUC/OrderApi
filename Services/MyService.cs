//using Authentication_and_Authorization_Api.Models;
using Microsoft.IdentityModel.Tokens;
using Minimart_Api.DTOS;
using Minimart_Api.Models;
using Minimart_Api.Mappings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Repositories;

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
        public async Task<IEnumerable<Users>> GetEntitiesAsync()
        {
            return await _repository.GetAllAsync();
        }
        //public async Task<ResponseStatus> CreateOrder(Order order) {

        //    return await _repository.CreateOrder(order);
        //}
      
     

       
        public async Task<IEnumerable<Users>> GetAsyncUserName(string UserName)
        {
            return await _repository.GetAsyncUserName(UserName);
        }



        //public async Task<UserRegStatus> UserRegistration(string JsonData)
        //{
        //    return await _repository.UserRegistration(JsonData);
        //}

        //public async Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname)
        //{
        //    return await _repository.GetDashBoardName(Dashboardname);
        //}

        //public async Task<IEnumerable<CategoryDTO>> GetDashBoardCategories()
        //{
        //    return await _repository.GetDashBoardCategories();
        //}






        public async Task<UserInfo> GetRefreshToken(string userID)
        {
            return await _repository.GetRefreshToken(userID);
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
