using Architecture.ViewModels.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services.Contracts
{
    public interface IProjectService
    {
        public Task<IndexProjectsViewModel> GetMyProjectsAsync(IndexProjectsViewModel model, string userId);
        public Task<DetailsProjectViewModel> GetProjectDetails(string id);

        public Task<string> CreateProjectAsync(CreateProjectViewModel model);

        public Task<int> DeleteProject(string id);
    }
}
