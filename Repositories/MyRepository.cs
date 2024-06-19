using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;

//using Minimart_Api.Data;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories
{
    public class MyRepository : IRepository
    {
        private readonly muchiriDBContext _dbContext;

        private readonly MyDBContext _myDBContext;
        //creates a constructor to get DBClass methods and propertis
        public MyRepository(muchiriDBContext dbContext, MyDBContext myDBContext) 
        {
            _dbContext = dbContext;

            _myDBContext = myDBContext;
        }

      

        public async Task<IEnumerable<TUser>> GetAllAsync() 
        {
            return await _dbContext.TUsers.ToListAsync();
                
        }
        public async Task<IEnumerable<TUser>> GetAsyncUserName(string UserName)
        {
            return await _dbContext.TUsers.Where(u => u.Name == UserName) .ToListAsync();

        }

        public async Task<Status> AddUsers(AddUser addUser) 
        { 
            return await Task.FromResult(_myDBContext.Database.SqlQuery<Status>(""));
        }
        
    }
}

