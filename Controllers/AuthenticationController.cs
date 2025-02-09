using Microsoft.AspNetCore.Mvc;
using Minimart_Api.Services;
using Minimart_Api.TempModels;

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
    }
}
