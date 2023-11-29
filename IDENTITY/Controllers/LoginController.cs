using Microsoft.AspNetCore.Mvc;

namespace IDENTITY.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
