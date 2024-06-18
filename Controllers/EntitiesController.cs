using Microsoft.AspNetCore.Mvc;
using Minimart_Api.Services;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IMyService _myService;
       
        public EntitiesController(IMyService myService)
        {
            _myService = myService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEntities()
        {
            var entities = await _myService.GetEntitiesAsync();

            return Ok(entities);
        }
    }
}
