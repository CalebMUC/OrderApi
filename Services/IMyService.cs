using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.Services
{
    public interface IMyService
    {
        Task<IEnumerable<TUser>> GetEntitiesAsync();
    }
}
