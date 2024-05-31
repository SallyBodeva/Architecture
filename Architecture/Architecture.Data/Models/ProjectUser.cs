using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Data.Models
{
    [PrimaryKey(nameof(ProjectId), nameof(UserId))]
    public class ProjectUser
    {
        public string ProjectId { get; set; }
        public string UserId { get; set; }
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
