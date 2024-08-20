namespace Architecture.Services
{
    using Data;
    using Services.Contracts;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNetCore.Identity;
    using Architecture.Data.Models;
    using Architecture.ViewModels.Projects;

    public class ProjectService : IProjectService
    {

        private ApplicationDbContext context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        public ProjectService(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
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

            return project.Id;

        }
    }
}
