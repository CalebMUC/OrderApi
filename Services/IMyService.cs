using Authentication_and_Authorization_Api.Models;
using Minimart_Api.DTOS;
using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Services
{
    public interface IMyService
    {
        Task<IEnumerable<TUser>> GetEntitiesAsync();

        Task<IEnumerable<TUser>> GetAsyncUserName(string UserName);

        Task<Status> AddToCart(string CartItems);

        Task<IEnumerable<CartResults>> GetCartItems(int UserID);

        Task<IEnumerable<TProduct>> FetchAllProducts();

        Task<ResponseStatus> AddProducts(AddProducts product);

        Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname);
        

        Task<IEnumerable<CartResults>> GetSubCategory(string CategoryName);

        Task<IEnumerable<CategoryDTO>> GetDashBoardCategories();


        Task<IEnumerable<CartResults>> GetProductsByCategory(string CategoryID);



        Task <UserInfo> GetRefreshToken(string JsonData);

        Task<IEnumerable<TProduct>> LoadProductImages(int ProductID);

        Task <UserInfo> Login(string JsonData);

        public void SaveRefreshToken(string JsonData);

        Task<UserRegStatus> UserRegistration(string JsonData);

        
    }
}
