using Architecture.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.ViewModels.Projects
{
    public class IndexProjectsViewModel : PagingViewModel
    {
        public IndexProjectsViewModel() : base(0)
        {
            
        }
        public IndexProjectsViewModel(int elementsCount, int itemsPerPage = 5, string action = "Index") : base(elementsCount, itemsPerPage, action)
        {
        }

        public List<IndexProjectViewModel> Projects { get; set; } = new List<IndexProjectViewModel>();
    }
}
