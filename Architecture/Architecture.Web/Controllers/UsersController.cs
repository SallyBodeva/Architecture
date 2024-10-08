﻿using Architecture.Services.Contracts;
using Architecture.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                return RedirectToAction("Index","Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userViewModel = await service.GetUserDetailsAsync(userId);
            if (userViewModel == null)
            {
                return NotFound();
            }

            return View(userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userViewModel = await service.GetUserToEditAsync(userId);
            if (userViewModel == null)
            {
                return NotFound();
            }

            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.EditUserAsync(model);
                return RedirectToAction("Details", "Users");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = service.DeleteUserAsync(userId);
            if (result.Result)
            {
                return View("Index");
            }
            return View(nameof(Details));
        }
    }
}
