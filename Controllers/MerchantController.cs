using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
using Minimart_Api.Services;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _mearchantService;
        public MerchantController(IMerchantService mearchantService) {
            _mearchantService = mearchantService;
        }
        [HttpGet("GetMerchants")]
        public async Task<IActionResult> GetMerchants()
        {

            try
            {
                var response = await _mearchantService.GetMerchantsAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("GetBusinessTypes")]
        public async Task<IActionResult> GetBusinessTypes()
        {

            try
            {
                var response = await _mearchantService.GetBusinessTypes();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("AddMerchants")]
        public async Task<IActionResult> AddMerchants(MerchantDTO merchantDTO)
        {

            try
            {
                var response = await _mearchantService.AddMerchants(merchantDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
