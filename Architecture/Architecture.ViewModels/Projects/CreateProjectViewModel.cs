using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.ViewModels.Projects
{
    public class CreateProjectViewModel
    {
        public string Name { get; set; }
        public string BuildingType { get; set; }

        public int Capacity { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int TotalFloorArea { get; set; }
        public int NumberOfFloors { get; set; }

        public string Address { get; set; }
        public string TownName { get; set; }
    }
}
