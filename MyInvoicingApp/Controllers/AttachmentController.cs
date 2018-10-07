using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Controllers
{
    public class AttachmentController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected IAttachmentManager AttachmentManager { get; set; }

        public AttachmentController(UserManager<ApplicationUser> userManager, IAttachmentManager attachmentManager)
        {
            UserManager = userManager;
            AttachmentManager = attachmentManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddJson(AttachmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.File != null)
                    {
                        await AttachmentManager.Add(model, CurrentUser);

                        return Json(new
                        {
                            success = true
                        });
                    }
                }
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
