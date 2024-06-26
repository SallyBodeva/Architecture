﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Architecture.Common;
using Architecture.Data;
using Architecture.Data.Models;
using Architecture.Services.Contracts;
using Architecture.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Architecture.Services
{

    public class UsersService : IUsersService
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;
        private const int ItemsCount = 0;

        public UsersService(UserManager<User> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<string> CreateUserAsync(CreateUserViewModel model)
        {
            string userAddress = model.Address;
            string userTown = model.Town;
            Town town = await context.Towns.FirstOrDefaultAsync(x=>x.Name==userTown);
            Address address = await context.Addresses.FirstOrDefaultAsync(x => x.Name == userAddress);
            if (town==null)
            {
                town = new Town() { Name = userTown };
                await context.Towns.AddAsync(town);
            }
            if (address==null)
            {
                address = new Address() { Name = userAddress, Town = town };
                await context.Addresses.AddAsync(address);
            }
            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                Address = address,
                PhoneNumber = model.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (userManager.Users.Count() <= 1)
                { 
                    IdentityRole roleAdmin = new IdentityRole() { Name = GlobalConstants.AdminRole };
                    IdentityRole roleClient = new IdentityRole() { Name = GlobalConstants.ClientRole };
                    IdentityRole roleEmployee = new IdentityRole() { Name = GlobalConstants.EmployeeRole };
                    await roleManager.CreateAsync(roleAdmin);
                    await roleManager.CreateAsync(roleEmployee);
                    await roleManager.CreateAsync(roleEmployee);
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdminRole);
                }
                else if(model.Role=="Employee")
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.EmployeeRole);
                }
                else
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.ClientRole);
                }
            }
            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            User? user = await GetUserByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return false;
        }



        public async Task<DetailsUserViewModel?> GetUserDetailsAsync(string id)
        {
            DetailsUserViewModel? result = null;

            User? user = await GetUserByIdAsync(id);

            if (user != null)
            {
                string roles = string.Join(", ", await userManager.GetRolesAsync(user));

                result = new DetailsUserViewModel()
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email != null ? user.Email : "n/a",
                    Phone = user.PhoneNumber != null ? user.PhoneNumber : "n/a",
                    Role = roles
                };
            }

            return result;
        }

        public async Task<IndexUsersViewModel> GetUsersAsync(IndexUsersViewModel model)
        {
            if (model == null)
            {
                model = new IndexUsersViewModel(0);
            }

            IQueryable<User> dataUsers = userManager.Users;

            if (!string.IsNullOrWhiteSpace(model.FilterByName))
            {
                dataUsers = dataUsers.Where(x => x.FirstName.Contains(model.FilterByName) || x.LastName.Contains(model.FilterByName));
            }
            //if (!string.IsNullOrWhiteSpace(model.FilterByRole))
            //{
            //    dataUsers = dataUsers.Where(x => string.Join("", x.Roles.Select(x=>x.)).Contains(model.FilterByRole));
            //}

            model.ElementsCount = await dataUsers.CountAsync();

            if (model.IsAsc)
            {
                model.IsAsc = false;
                if (model.SortUsersBy == "Name")
                {
                    dataUsers = dataUsers.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName);
                }
                else
                {
                    //dataUsers = dataUsers.OrderByDescending(x =>x.Roles.FirstOrDefault().RoleId);
                }
            }
            else
            {
                model.IsAsc = true;
                if (model.SortUsersBy == "Name")
                {
                    dataUsers = dataUsers.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
                }
                else
                {
                    //dataUsers = dataUsers.OrderBy(x => x.Roles.FirstOrDefault().RoleId);
                }
            }

            model.Users = await dataUsers
                .Skip((model.Page - 1) * model.ItemsPerPage)
                .Take(model.ItemsPerPage)
                .Select(x => new IndexUserViewModel()
                {
                    Id = x.Id,
                    Name = $"{x.FirstName} {x.LastName}",
                    Role = string.Join(", ", userManager.GetRolesAsync(x).GetAwaiter().GetResult())
                })
                .ToListAsync();

            return model;
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await userManager.Users.CountAsync();
        }

        public async Task<EditUserViewModel?> GetUserToEditAsync(string id)
        {
            EditUserViewModel? result = null;

            User? user = await GetUserByIdAsync(id);

            if (user != null)
            {
                result = new EditUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
            }

            return result;


        }

        public async Task<string> UpdateUserAsync(EditUserViewModel user)
        {
            User? oldUser = await GetUserByIdAsync(user.Id);

            if (oldUser != null)
            {
                oldUser.FirstName = user.FirstName;
                oldUser.LastName = user.LastName;
                await userManager.UpdateAsync(oldUser);
            }

            return user.Id;
        }

        private async Task<User?> GetUserByIdAsync(string id)
        {
            return await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<SignInResult> Login(LoginViewModel model)
        {
            return await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        }
    }
}
