using Microsoft.EntityFrameworkCore;
using Minimart_Api.Models;

namespace Minimart_Api.Data
{
    public class MyDBContext : DbContext
    {
        public DbSet<AddUser> MyStoredProcedureResults { get; set; }

        public MyDBContext() : base("name = p_AddUser")
        {
        }
    }
}
