using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Data.Models
{
    public  class Project
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string? Description { get; set; }
        public string BuildingType { get; set; }
        public int Capacity { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int TotalFloorArea { get; set; }
        public int NumberOfFloors { get; set; }
        public string? Image { get; set; }
        public string AddressId { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
