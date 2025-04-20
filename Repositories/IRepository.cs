 using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<Users>> GetAllAsync();

        Task<IEnumerable<Users>> GetAsyncUserName(string username);


        

        //Task<UserRegStatus> UserRegistration(string CartItems);

        //Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname);

        //Task<IEnumerable<CategoryDTO>> GetDashBoardCategories();

        

        

        Task<UserInfo> GetRefreshToken(string Categoryname);


        

        

        


        //Task<ResponseStatus> CreateOrder(Order order);

        //public Task<ResponseStatus> AddOrder(OrderListDto orderDTO);

       
        



        //Task <LoginResponse> Login(string JsonData);

        public Task SaveRefreshToken(string JsonData);
    }
}
