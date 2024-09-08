using Authentication_and_Authorization_Api.Models;
using Minimart_Api.DTOS;
using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<TUser>> GetAllAsync();

        Task<IEnumerable<TUser>> GetAsyncUserName(string username);

        Task<Status> AddToCart(string CartItems);

        Task<UserRegStatus> UserRegistration(string CartItems);
        
        Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname);

        Task<IEnumerable<CategoryDTO>> GetDashBoardCategories();

        Task<IEnumerable<CartResults>> GetSubCategory(string Categoryname);

        Task<IEnumerable<CartResults>> GetProductsByCategory(string CategoryID);

        Task <UserInfo> GetRefreshToken(string Categoryname);



        Task<IEnumerable<TProduct>> LoadProductImages(int ProductID);

        Task<IEnumerable<CartResults>> GetCartItems(int UserID);

        Task<IEnumerable<TProduct>> FetchAllProducts();

        Task<ResponseStatus> AddProducts(AddProducts product);


        Task <UserInfo> Login(string JsonData);

        public void SaveRefreshToken(string JsonData);
    }
}
