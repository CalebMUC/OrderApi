using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Repositories.Authorization;
using Minimart_Api.Models;

namespace Minimart_Api.Services
{
    public class AuthenticationService: IAuthentication
    {
        private readonly IAuthRepository _authRepository;
        public AuthenticationService(IAuthRepository authRepository ) {

            _authRepository = authRepository;
        }

        public async Task<RegisterResponse> Register(Register register) { 

          return  await _authRepository.Register(register);
        }

        public async Task<LoginResponse> Login(UserLogin userLogin)
        {

            return await _authRepository.Login(userLogin);
        }
    }
}
