using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<TUser>> GetAllAsync();
    }
}
