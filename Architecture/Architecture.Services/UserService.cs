namespace Architecture.Services
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
                Role = model.Role
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
    }
}
