using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Data.Models
{
    public class ProjectUser
    {
        public string UserId { get; set; }
        public string ProjectId { get; set; }

        public virtual User User { get; set; }
        public virtual Project  Project { get; set; }
    }
}
