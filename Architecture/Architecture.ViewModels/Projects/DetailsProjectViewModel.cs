using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.ViewModels.Projects
{
    public class DetailsProjectViewModel
    {
        public string Name { get; set; }
        public string BuldingType { get; set; }
        public int Capacity { get; set; }
        public string ReleaseDate { get; set; }
        public int TotalFloorArea { get; set; }
        public int NumberOfFloors { get; set; }
        public string AddressName { get; set; }

        public string Town { get; set; }
    }
}
