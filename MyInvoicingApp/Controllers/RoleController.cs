using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin")]
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
            try
            {
                var roles = await RoleManager.Roles.Select(x => new RoleViewModel(x)).ToListAsync();
                return View(roles);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            try
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
                        TempData["Success"] = $"Dodano nową rolę/stanowisko <b> {newRole.Name} </b>";

                        return RedirectToAction("Index", "Role");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                var role = RoleManager.Roles.SingleOrDefault(x => x.Id == id);

                if (role == null)
                {
                    throw new ArgumentNullException(nameof(id), "Nie pobrano żadnej roli dla podanego Id");
                }

                return View(new RoleViewModel()
                {
                    Position = role.Name,
                    Description = role.Description,
                    Id = role.Id
                });
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            try
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
                        TempData["Success"] = $"Zapisano wprowadzone zmiany dla roli/stanowiska <b> {role.Name} </b>";

                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Close(string id)
        {
            try
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

                TempData["Success"] = $"Rola/stanowisko zostało <b> {role.Name} </b> zamknięte";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ReOpen(string id)
        {
            try
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

                TempData["Success"] = $"Rola/stanowisko <b> {role.Name} </b> zostało otwarte";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AsignUser(string id)
        {
            try
            {
                var role = await RoleManager.FindByIdAsync(id);

                if (role == null)
                {
                    throw new Exception("Nie istnieje role o podanym Id.");
                }

                var asignedUsers = await UserManager.GetUsersInRoleAsync(role.Name);
                var otherUsers = UserManager.Users.Except(asignedUsers);

                ViewBag.otherUsers = otherUsers.ToList().Select(x => new SelectListItem(){ Text = x.UserName, Value = x.Id });
                ViewBag.asignedUsers = asignedUsers.ToList().Select(x => new SelectListItem() { Text = x.UserName, Value = x.Id });

                return View(new RoleViewModel(role)
                {
                    AsignedUsers = asignedUsers,
                });
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string id, string[] userId)
        {
            var asignedUsersSb = new StringBuilder("");

            try
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
                        throw new Exception($"Nie udało się dodać użytkownika {user.UserName} do roli {role.Name}");
                    }

                    asignedUsersSb.Append($", {user.UserName}");
                }

                TempData["Success"] = $"Przypisano uzytkowników <b> {asignedUsersSb.ToString().Substring(2)} </b> do roli {role.Name}";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string id, string[] userId)
        {
            var removedUsersSb = new StringBuilder("");

            try
            {
                var role = await RoleManager.FindByIdAsync(id);

                if (!userId.Any())
                {
                    throw new ArgumentException("Nie podano żadnego użytkownika", nameof(role));
                }

                foreach (var uid in userId)
                {
                    var user = await UserManager.FindByIdAsync(uid);
                    var result = await UserManager.RemoveFromRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Nie udało się usunąć użytkownika {user.UserName} z roli {role.Name}");
                    }

                    removedUsersSb.Append($", {user.UserName}");
                }

                TempData["Success"] = $"Usunięto uzytkowników <b> {removedUsersSb.ToString().Substring(2)} </b> z roli {role.Name}";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }
    }
}
