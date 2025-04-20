using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.Services.SystemMerchantService;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemMerchantController : ControllerBase
    {
        private readonly ISystemMerchants _systemMerchants;
      public SystemMerchantController(ISystemMerchants systemMerchants) {
            _systemMerchants = systemMerchants;
        }
        [HttpGet("GetAllMerchants")]
        public  async Task<IActionResult> GetAllMerchantsAsync()
        {
            try { 
                var response = await _systemMerchants.GetAllMerchantsAsync();
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMerchantById")]
        public async Task<IActionResult> GetMerchantByIdAsync(int merchantID)
        {
            try
            {
                var response = await _systemMerchants.GetMerchantByIdAsync(merchantID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddMerchantsAsync")]
        public async Task<IActionResult> AddMerchantsAsync(SystemMerchantsDto systemMerchantsDto)
        {
            try
            {
                var response = await _systemMerchants.AddMerchantsAsync(systemMerchantsDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateMerchantsAsync")]
        public async Task<IActionResult> UpdateMerchantsAsync(SystemMerchantsDto systemMerchantsDto)
        {
            try
            {
                var response = await _systemMerchants.UpdateMerchantsAsync(systemMerchantsDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("DeleteMerchantAsync")]
        public async Task<IActionResult> DeleteMerchantAsync(int merchantId)
        {
            try
            {
                var response = await _systemMerchants.DeleteMerchantAsync(merchantId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
