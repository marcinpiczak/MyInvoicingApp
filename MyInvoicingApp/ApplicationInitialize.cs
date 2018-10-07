using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyInvoicingApp.Models;

namespace MyInvoicingApp
{
    public class ApplicationInitialize
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected RoleManager<ApplicationRole> RoleManager { get; set; }

        //public AppInit(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        //{
        //    UserManager = userManager;
        //    RoleManager = roleManager;
        //}

        public ApplicationInitialize(IServiceProvider services)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            }
        }

        public async Task Init()
        {
            await AsignAdminToAdminRoleAsync();
            await CreateRoleAccountantAsync();
            await CreateRoleManagerAsync();

        }

        private async Task<ApplicationUser> CreateUserAdminAsync()
        {
            var admin = UserManager.Users.SingleOrDefault(x => x.UserName == "Admin");

            if (admin == null)
            {
                admin = new ApplicationUser("Admin");
                var result = await UserManager.CreateAsync(admin, "123456");

                if (!result.Succeeded)
                {
                    throw new Exception("Nie można utworzyć użtkownika Admin");
                }
            }

            return admin;
        }

        private async Task<ApplicationRole> CreateRoleAccountantAsync()
        {
            return await CreateRoleAsync("Accountant", "Księgowy");
        }

        private async Task<ApplicationRole> CreateRoleAdminAsync()
        {
            return await CreateRoleAsync("Admin", "Administrator");
        }

        private async Task<ApplicationRole> CreateRoleManagerAsync()
        {
            return await CreateRoleAsync("Manager", "Kierownik");
        }

        private async Task<ApplicationRole> CreateRoleAsync(string roleName, string description)
        {
            var role = RoleManager.Roles.SingleOrDefault(x => x.Name == roleName);

            if (role == null)
            {
                role = new  ApplicationRole(roleName)
                {
                    Description = description
                };

                var result = await RoleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new Exception($"Nie można utworzyć role {roleName}");
                }
            }

            return role;
        }

        private async Task AsignAdminToAdminRoleAsync()
        {
            var userAdmin = await CreateUserAdminAsync();
            var userRole = await CreateRoleAdminAsync();

            var result = await UserManager.AddToRoleAsync(userAdmin, userRole.Name);

            if (!result.Succeeded)
            {
                throw new Exception("Nie udało się przypisać użytkownika Admin do roli Admin");
            }
        }
    }
}