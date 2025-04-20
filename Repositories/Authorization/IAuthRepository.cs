using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.Authorization
{
    public interface IAuthRepository
    {
        public Task<RegisterResponse> Register(Register register);

        public Task<LoginResponse> Login(UserLogin login);
    }
}
