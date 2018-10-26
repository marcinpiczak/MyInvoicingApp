using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin,Accountant,Manager")]
    public class AttachmentController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected IAttachmentManager AttachmentManager { get; set; }
        protected IHostingEnvironment HostingEnvironment;

        public AttachmentController(UserManager<ApplicationUser> userManager, IAttachmentManager attachmentManager, IHostingEnvironment hostingEnvironment)
        {
            UserManager = userManager;
            AttachmentManager = attachmentManager;
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Add attachments to Attachments folder and creates entry in attachments table in database
        /// </summary>
        /// <param name="model">An AttachmentViewModel with all data passed from form: file and description</param>
        /// <returns>JSON with status of attachment addition and array of error if any occure</returns>
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

        /// <summary>
        /// Removes attachment from Attachment folder and changes status of database entry to Closed
        /// </summary>
        /// <param name="attachmentId">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>JSON with status of attachment deletion and array of error if any occure</returns>
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

        /// <summary>
        /// Gets attachment file for given attachment id, document type and document Id
        /// </summary>
        /// <param name="attachmentId">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>Attachment file for given attachment id, document type and document Id</returns>
        [HttpGet]
        public IActionResult Get(string attachmentId, DocumentType documentType, string documentId)
        {
            try
            {
                var attachment = AttachmentManager.GetAttachmentById(attachmentId, documentType, documentId, true);

                var path = HostingEnvironment.ContentRootPath;

                //return File(attachment.FilePath.Replace("wwwroot", ""), attachment.ContentType, attachment.OriginalFileName);
                //return File(path + @"/Attachments/26-trp9s_88755675-7ad6-4aa3-9bcf-056b8ad84098.jpg", "image/jpeg", "plik");
                return PhysicalFile(Path.Combine(path, attachment.FilePath), attachment.ContentType, attachment.OriginalFileName);
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
