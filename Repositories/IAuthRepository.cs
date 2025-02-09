using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories
{
    public interface IAuthRepository
    {
        public Task<RegisterResponse> Register(Register register);

        public Task<LoginResponse> Login(UserLogin login);
    }
}
