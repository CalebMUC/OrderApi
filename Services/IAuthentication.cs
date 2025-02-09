using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface IAuthentication
    {
        public Task<RegisterResponse> Register(Register register);
        public Task<LoginResponse> Login(UserLogin login);
    }
}
