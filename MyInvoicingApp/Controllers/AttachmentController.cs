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
                        var attachment = await AttachmentManager.Add(model, CurrentUser);

                        return Json(new
                        {
                            success = true,
                            Id = attachment.Id,
                            DocumentId = attachment.DocumentId,
                            DocumentType = attachment.DocumentType.ToString(),
                            FileName = attachment.OriginalFileName,
                            FileDescription = attachment.FileDescription
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

        [HttpPost]
        public IActionResult DeleteJson(string attachmentId, DocumentType documentType, string documentId)
        {
            try
            {
                AttachmentManager.RemoveAttachmentForDocumentById(attachmentId, documentType, documentId);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";

                return Json(new
                {
                    success = false,
                    errors = new [] { $"{e.Message}: {innerMessage}" }
                });
            } 
        }

        [HttpGet]
        public IActionResult Get(string attachmentId, DocumentType documentType, string documentId)
        {
            try
            {
                var attachment = AttachmentManager.GetAttachmentById(attachmentId, documentType, documentId, true);

                return File(attachment.FilePath.Replace("wwwroot", ""), attachment.ContentType, attachment.OriginalFileName);
                //return File(@"\Attachments\26-trp9s_73a580ec-2cd7-4e2f-a34e-455ec2ead947.jpg", "image/jpeg", "plik");
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas pobierania załącznika: {e.Message}{innerMessage}";

                if (documentType == DocumentType.Invoice)
                {
                    return RedirectToAction("Index", "Invoice");
                }
                if (documentType == DocumentType.Budget)
                {
                    return RedirectToAction("Index", "Budget");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                //return Json(new
                //{
                //    success = false,
                //    errors = new[] { $"{e.Message}: {innerMessage}" }
                //});
            }
        }
    }
}
