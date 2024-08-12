using Microsoft.AspNetCore.Mvc;

namespace Architecture.Web.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
