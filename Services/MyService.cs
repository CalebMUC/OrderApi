using Minimart_Api.Models;
using Minimart_Api.Repositories;

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
    }
}
