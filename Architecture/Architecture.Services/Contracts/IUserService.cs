﻿using Architecture.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services.Contracts
{
    public interface IUserService
    {
        public Task<string> CreateUserAsync(CreateUserViewModel model);
        public Task<DetailsUserViewModel?> GetUserDetailsAsync(string id);
    }
}
