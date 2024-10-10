using Minimart_Api.DTOS;

namespace Minimart_Api.Repositories
{
    public interface IorderRepository
    {
        Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status,int userID); // This is the correct interface method declaration


    }
}
