using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Architecture.Data;
using Architecture.Data.Models;
using Architecture.ViewModels.Projects;
using Architecture.Services.Contracts;
using System.Security.Claims;

namespace Architecture.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectService projectService;

        public ProjectsController(ApplicationDbContext context, IProjectService service)
        {
            _context = context;
            projectService = service;
        }

        // GET: Projects
        public async Task<IActionResult> MyProjects(IndexProjectsViewModel model)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            model = await projectService.GetMyProjectsAsync(model, userId);

            return View(model);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await projectService.GetProjectDetails(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await projectService.CreateProjectAsync(model);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }



        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Address)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await projectService.DeleteProject(id);
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
