using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<TUser>> GetAllAsync();

        Task<IEnumerable<TUser>> GetAsyncUserName(string username);

        Task<Status> AddUsers(AddUser addUser);
    }
}
