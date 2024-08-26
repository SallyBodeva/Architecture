namespace Architecture.Services
{
    using Data;
    using Services.Contracts;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNetCore.Identity;
    using Architecture.Data.Models;
    using Architecture.ViewModels.Projects;
    using Microsoft.EntityFrameworkCore;

    public class ProjectService : IProjectService
    {

        private ApplicationDbContext context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        public ProjectService(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IndexProjectsViewModel> GetMyProjectsAsync(IndexProjectsViewModel model,string userId)
        {
            if (model == null)
            {
                model = new IndexProjectsViewModel();
            }
            model.ElementsCount = await GetProjectCountAsync();
            model.Projects = await context
            .ProjectsUsers
                .Skip((model.Page - 1) * model.ItemsPerPage)
                .Take(model.ItemsPerPage)
                .Where(x=>x.UserId==userId)
                .Select(x => new IndexProjectViewModel()
                {
                    Id = x.Project.Id,
                    Name = x.Project.Name,
                    BuildingType = x.Project.BuilindType,
                    ReleaseDate = x.Project.ReleaseDate.Value.ToShortDateString(),

                })
                .ToListAsync();

            return model;
        }
        public async Task<int> GetProjectCountAsync()
        {
            return await context.Projects.CountAsync();
        }

        public async Task<string> CreateProjectAsync(CreateProjectViewModel model)
        {
            Address address = context.Addresses.FirstOrDefault(x => x.Name == model.Name);
            if (address == null)
            {
                Town town = context.Towns.FirstOrDefault(x => x.Name == model.TownName);
                if (town == null)
                {
                    town = new Town() { Name = model.TownName };
                    context.Towns.Add(town);
                    context.SaveChanges();
                }
                address = new Address() { Name = model.Name, Town = town };
                context.Addresses.Add(address);
                context.SaveChanges();
            }

            Project project = new Project()
            {
                Name = model.Name,
                BuilindType = model.BuildingType,
                Capacity = model.Capacity,
                ReleaseDate = model.ReleaseDate,
                TotalFloorArea = model.TotalFloorArea,
                NumberOfFloors = model.NumberOfFloors,
                Address = address
            };
            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            User? user = await this.context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user != null && project != null)
            {
                ProjectUser match = new ProjectUser()
                {
                    UserId = user.Id,
                    ProjectId = project.Id,
                    Type = "Client - Company"
                };
                this.context.ProjectsUsers.Add(match);
                await this.context.SaveChangesAsync();
            }

            return project.Id;

        }

    }
}
