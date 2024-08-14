using Architecture.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.ViewModels.Users
{
    public class DetailsUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }

        public string? Department  { get; set; }

        public string Role { get; set; }


        public Dictionary<string, Project>? Projects { get; set; } = new Dictionary<string, Project>();
    }
}
