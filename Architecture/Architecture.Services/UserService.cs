﻿namespace Architecture.Services
{
    using Common;
    using Data;
    using Data.Models;
    using Services.Contracts;
    using ViewModels.Users;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;

    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;
        private const int ItemsCount = 0;

        public UserService(UserManager<User> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<string> CreateUserAsync(CreateUserViewModel model)
        {

            Address address = this.context.Addresses.FirstOrDefault(x => x.Name == model.AddressName);
            if (address == null)
            {
                Town town = context.Towns.FirstOrDefault(x => x.Name == model.TownName);
                if (town == null)
                {
                    town = new Town() { Name = model.TownName };
                    context.Towns.Add(town);
                    context.SaveChanges();
                }
                address = new Address { Name = model.AddressName, TownId = town.Id };
                context.Addresses.Add(address);
                context.SaveChanges();
            }
            User user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = address,
                Role = model.Role,
                Department = model.Department,
                ProfileImage = await ImageToStringAsync(model.File),

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
                    await roleManager.CreateAsync(roleClient);
                    await roleManager.CreateAsync(roleEmployee);
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdminRole);
                    user.Role = GlobalConstants.AdminRole;
                }
                else
                {
                    if (model.Role == "Client")
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.ClientRole);
                        user.Role = GlobalConstants.ClientRole;
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.EmployeeRole);
                        user.Role = GlobalConstants.EmployeeRole;
                    }

                }
                await userManager.UpdateAsync(user);
            }
            return user.Id;
        }
        private async Task<string> ImageToStringAsync(IFormFile file)
        {
            List<string> imageExtensions = new List<string>() { ".JPG", ".BMP", ".PNG" };


            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (imageExtensions.Contains(extension.ToUpperInvariant()))
                {
                    using var dataStream = new MemoryStream();
                    await file.CopyToAsync(dataStream);
                    byte[] imageBytes = dataStream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            return null;
        }
        public async Task<DetailsUserViewModel?> GetUserDetailsAsync(string id)
        {
            DetailsUserViewModel? result = null;

            User user = await userManager.FindByIdAsync(id);

            Dictionary<string, Project> neededInfoForProjects = new Dictionary<string, Project>();

            List<Project> userProjects = context.Users
              .FirstOrDefault(x => x.Id == id).ProjectUsers.Select(x => x.Project).ToList();


            for (int i = 0; i < userProjects.Count; i++)
            {
                neededInfoForProjects.Add(userProjects[i].Name, userProjects[i]);
            }

            if (user != null)
            {

                result = new DetailsUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email != null ? user.Email : "n/a",
                    Address = user.Address.Name,
                    Town = user.Address.Town.Name,
                    Department = user.Department,
                    Role = user.Role,
                    Image = user.ProfileImage,
                    Projects = neededInfoForProjects
                };
            }
            return result;
        }
        public async Task<EditUserViewModel?> GetUserToEditAsync(string id)
        {
            EditUserViewModel result = new EditUserViewModel();

            User? user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                result.Id = user.Id;
                result.FirstName = user.FirstName;
                result.LastName = user.LastName;
                result.Email = user.Email;
                result.PhoneNumber = user.PhoneNumber;
                result.Department = user.Department;
                result.Role = user.Role;
            }

            return result;
        }

        public async Task<string> EditUserAsync(EditUserViewModel model)
        {
            User? oldUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (oldUser != null)
            {
                oldUser.FirstName = model.FirstName;
                oldUser.LastName = model.LastName;
                oldUser.Email = model.Email;
                oldUser.UserName = model.Email;
                oldUser.PhoneNumber = model.PhoneNumber;
                oldUser.Department = model.Department;
                oldUser.ProfileImage = await ImageToStringAsync(model.Image);

                context.Users.Update(oldUser);
                await userManager.UpdateAsync(oldUser);
                await context.SaveChangesAsync();
            }

            return oldUser.Id;
        }
        public async Task<bool> DeleteUserAsync(string id)
        {
            User? user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

            List<ProjectUser> unfinishedProjects = user.ProjectUsers.Where(x => x.Project.ReleaseDate < DateTime.UtcNow).ToList();

            if (user != null && (unfinishedProjects.Count == 0))
            {
                var result = await userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return false;
        }
    }
}
