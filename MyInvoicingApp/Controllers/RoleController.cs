using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyInvoicingApp.Controllers
{
    public class RoleController : Controller
    {
        protected RoleManager<ApplicationRole> RoleManager;
        protected UserManager<ApplicationUser> UserManager;

        private string[] _systemRoles => new[]
        {
            "Accountant",
            "Manager",
            "Admin"
        };

        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await RoleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newRole = new ApplicationRole(model.Position)
                {
                    Description = model.Description
                };

                var result = await RoleManager.CreateAsync(newRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Role");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var role = RoleManager.Roles.SingleOrDefault(x => x.Id == id);

            if (role == null)
            {
                throw new ArgumentNullException();
            }

            return View(new RoleViewModel()
            {
                Position = role.Name,
                Description = role.Description,
                Id = role.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = RoleManager.Roles.SingleOrDefault(x => x.Id == model.Id);

                if (role == null)
                {
                    throw new ArgumentNullException();
                }

                role.Name = model.Position;
                role.Description = model.Description;

                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Close(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new ArgumentException("Nie istnieje role o podanym Id", nameof(role));
            }

            if (_systemRoles.Contains(role.Name))
            {
                throw new Exception("Nie można zamknąć ról systemowych");
            }
            
            var usersInRole = await UserManager.GetUsersInRoleAsync(role.Name);

            if (usersInRole.Any())
            {
                throw new ArgumentException("Nie można zamknąć roli do której przypisani są użytkownicy", nameof(role));
            }

            role.Status = Status.Closed;
            var result = await RoleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new ArgumentException("Nie udało się zamknąć roli", nameof(role));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ReOpen(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new ArgumentException("Nie istnieje role o podanym Id", nameof(role));
            }

            role.Status = Status.Opened;
            var result = await RoleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new ArgumentException("Nie udało się ponownie otworzyć roli", nameof(role));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AsignUser(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new Exception("Nie istnieje role o podanym Id.");
            }

            var asignedUsers = await UserManager.GetUsersInRoleAsync(role.Name);
            var otherUsers = UserManager.Users.Except(asignedUsers);

           ViewBag.otherUsers = otherUsers.ToList().Select(x => new SelectListItem(){Text = x.UserName, Value = x.Id});

            return View(new RoleViewModel(role)
            {
                AsignedUsers = asignedUsers,
            });
        }

        [HttpPost]
        public async Task<IActionResult> AsignUser(string id, string[] userId)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new ArgumentException("Nie istnieje role o podanym Id", nameof(role));
            }

            if (!userId.Any())
            {
                throw new ArgumentException("Nie podano żadnego użytkownika", nameof(role));
            }

            foreach (var uid in userId)
            {
                var user = await UserManager.FindByIdAsync(uid);
                var result = await UserManager.AddToRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    Console.WriteLine($"Nie udało się dodać użytkownika {user.UserName} do roli {role.Name}");
                }
            }

            return RedirectToAction("Index", "Role");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromRole(string id, string userId)
        {
            var role = await RoleManager.FindByIdAsync(id);
            var user = await UserManager.FindByIdAsync(userId);

            var result = await UserManager.RemoveFromRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                throw new Exception($"Nie udało się usunąć użytkownika {user.UserName} od roli {role.Name}");
            }

            return RedirectToAction("Index", "Role");
        }
    }
}
