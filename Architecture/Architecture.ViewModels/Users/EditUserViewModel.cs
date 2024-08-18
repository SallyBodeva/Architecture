using Architecture.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string? Id { get; set; }
        public string? Role { get; set; }
        [BindProperty]
        public IFormFile? Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Department { get; set; }
    }
}
