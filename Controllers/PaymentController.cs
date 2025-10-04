using Microsoft.AspNetCore.Mvc;

namespace Minimart_Api.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
