using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.Services.Address;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IAddress _addressService;
        public AddressController(IAddress address) {
            _addressService = address;

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetAddressesByUserId(int userId)
        //{
        //    var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
        //    return Ok(addresses);
        //}


        [HttpGet("GetAddressesByUserId/{userId}")]
        public async Task<IActionResult> GetAddressesByUserId(int userId)
        {
            try
            {

                var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    responseCode = 500,
                    responseMessage = "An error occurred while fetching addresses.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDTO address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    responseCode = 400,
                    responseMessage = "Invalid address data",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            var result = await _addressService.AddAddressAsync(address);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    responseCode = 400,
                    responseMessage = result.Message
                });
            }

            var updatedAddresses = await _addressService.GetAddressesByUserIdAsync(address.UserID);

            return Ok(new
            {
                responseCode = 200,
                responseMessage = "Address added successfully",
                addresses = updatedAddresses
            });
        }


        [HttpPost("EditAddress")]
        public async Task<IActionResult> EditAddress([FromBody] EditAddressDTO address)
        {
            try
            {
                await _addressService.EditAddressAsync(address);

                // Fetch the updated list of addresses for the user
                var updatedAddresses = await _addressService.GetAddressesByUserIdAsync(address.UserID);

                return Ok(new
                {
                    responseCode = 200,
                    responseMessage = "Address updated successfully.",
                    addresses = updatedAddresses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    responseCode = 500,
                    responseMessage = "An error occurred while updating the address.",
                    error = ex.Message
                });
            }
        }
    }
}
