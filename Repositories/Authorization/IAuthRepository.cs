using Minimart_Api.DTOS.Authorization;
using Minimart_Api.DTOS.General;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.Authorization
{
    public interface IAuthRepository
    {
        public Task<RegisterResponse> Register(Register register);

        public Task<LoginResponse> Login(UserLogin login);

        public Task<Status> SendEmailVerificationCode(string email);
        public Task<Status> SendResetCode(PasswordResetDto passwordReset);
        public  Task<Status> VerifyResetCode(VerifyResetCodeDto verifyResetCode);
        public Task<Status> VerifyEmailValidationCode(EmailVerificationCodeDTO verificationCodeDTO);
        public Task<Status> ResetPassword(ResetPasswordDto resetPassword);
    }
}
