using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Repositories.Authorization;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;

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

        public async Task<Status> SendResetCode(PasswordResetDto passwordReset) {

            return await _authRepository.SendResetCode(passwordReset);
        }
        public async Task<Status> VerifyResetCode(VerifyResetCodeDto verifyResetCode) {

            return await _authRepository.VerifyResetCode(verifyResetCode);
        }

        public async Task<Status> VerifyEmailValidationCode(EmailVerificationCodeDTO verificationCodeDTO)
        {

            return await _authRepository.VerifyEmailValidationCode(verificationCodeDTO);
        }
        public async Task<Status> ResetPassword(ResetPasswordDto resetPassword) {

            return await _authRepository.ResetPassword(resetPassword);
        }

        public async Task<Status> SendEmailVerificationCode(string email) {

            return await _authRepository.SendEmailVerificationCode(email);
        }
    }
}
