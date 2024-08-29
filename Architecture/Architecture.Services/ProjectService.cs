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

        public async Task<IndexProjectsViewModel> GetMyProjectsAsync(IndexProjectsViewModel model, string userId)
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
                .Where(x => x.UserId == userId)
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
        public async Task<DetailsProjectViewModel> GetProjectDetails(string id)
        {
            Project? project = await this.context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            DetailsProjectViewModel model = null;

            if (project != null)
            {
                model = new DetailsProjectViewModel()
                {
                    Name = project.Name,
                    BuldingType = project.BuilindType,
                    Capacity = project.Capacity,
                    ReleaseDate = project.ReleaseDate.Value.ToShortDateString(),
                    TotalFloorArea = project.TotalFloorArea,
                    NumberOfFloors = project.NumberOfFloors,
                    AddressName = project.Address.Name,
                    Town = project.Address.Town.Name
                };

            }
            return model;
        }
        public async Task<int> DeleteProject(string id)
        {
            Project p = await context.Projects.FirstOrDefaultAsync(x=>x.Id==id);
            ProjectUser connection = await context.ProjectsUsers.FirstOrDefaultAsync(x => x.ProjectId == p.Id);
            
            context.ProjectsUsers.Remove(connection);
            context.Projects.Remove(p);
            return await context.SaveChangesAsync();
        }

        // TODO
    }
}
