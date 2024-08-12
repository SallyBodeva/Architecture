using Architecture.Services.Contracts;
using Architecture.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace Architecture.Web.Controllers
{
    public class UsersController : Controller
    {
        private IUserService service;

        public UsersController(IUserService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.CreateUserAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
