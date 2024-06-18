using Microsoft.EntityFrameworkCore;
//using Minimart_Api.Data;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories
{
    public class MyRepository : IRepository
    {
        private readonly muchiriDBContext _dbContext;
        //creates a constructor to get DBClass methods and propertis
        public MyRepository(muchiriDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TUser>> GetAllAsync() 
        {
            return await _dbContext.TUsers.ToListAsync();
                
        }
    }
}
