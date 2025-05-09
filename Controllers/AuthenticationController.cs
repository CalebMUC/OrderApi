using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Services;
using Minimart_Api.Models;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication _authentication;
        public AuthenticationController(IAuthentication authentication) {

            _authentication = authentication;
        }
        [HttpPost("Register")]

        public async Task<IActionResult> Register([FromBody] Register register)
        {
            try
            {
                var response = await _authentication.Register(register);

                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserLogin login)
        {
            try
            {
               var response = await _authentication.Login(login);

                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("SendResetCode")]
        public async Task<IActionResult> SendResetCode([FromBody] PasswordResetDto passwordReset)
        {
            try
            {
                var response = await _authentication.SendResetCode(passwordReset);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("VerifyEmailValidationCode")]
        public async Task<IActionResult> VerifyEmailValidationCode([FromBody] EmailVerificationCodeDTO verificationCodeDTO)
        {
            try
            {
                var response = await _authentication.VerifyEmailValidationCode(verificationCodeDTO);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("VerifyResetCode")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeDto verifyResetCode)
        {
            try
            {
                var response = await _authentication.VerifyResetCode(verifyResetCode);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            try
            {
                var response = await _authentication.ResetPassword(resetPassword);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("SendEmailVerificationCode")]
        public async Task<IActionResult> SendEmailVerificationCode([FromBody] EmailVerificationDto emailVerification)
        {
            try
            {
                var response = await _authentication.SendEmailVerificationCode(emailVerification.Email);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
