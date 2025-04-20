using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Models;

namespace Minimart_Api.Services
{
    public interface IAuthentication
    {
        public Task<RegisterResponse> Register(Register register);
        public Task<LoginResponse> Login(UserLogin login);
    }
}
