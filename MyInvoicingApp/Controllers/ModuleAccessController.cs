using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModuleAccessController : Controller
    {
        protected IModuleAccessManager ModuleAccessManager { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);

        private List<Models.Controllers> _availableModules { get; } = new List<Models.Controllers>()
        {
            Models.Controllers.Budget,
            Models.Controllers.Customer,
            Models.Controllers.Invoice
        };

        public ModuleAccessController(IModuleAccessManager moduleAccessManager, UserManager<ApplicationUser> userManager)
        {
            ModuleAccessManager = moduleAccessManager;
            UserManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Role");
        }

        [HttpGet]
        public IActionResult ChangeRoleAccess(string roleId)
        {
            var roleAccesses = ModuleAccessManager.GetRoleModuleAccessViewModels(roleId);

            var roleModules = roleAccesses.Select(x => x.Module).Distinct();

            var otherModulesList = _availableModules.Except(roleModules);

            var otherModules = otherModulesList.Select(x => new ModuleAccessViewModel()
            {
                AccessorType = AccessorType.Role,
                AccessorId = roleId,
                Module = x
            });

            roleAccesses = roleAccesses.Union(otherModules);

            return View(roleAccesses);
        }

        [HttpPost]
        public IActionResult ChangeRoleAccessJson([FromBody]IEnumerable<ModuleAccessViewModel> model)
        {
            try
            {
                ModuleAccessManager.SaveRoleModuleAccesses(model, CurrentUser);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
            }

            return Json(new
            {
                success = false,
                errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                    .Select(m => m.ErrorMessage).ToArray()
            });
        }
    }
}
