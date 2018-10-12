using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Controllers
{
    [Authorize]
    //[DisplayName("Faktury")]
    public class InvoiceController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected IInvoiceManager InvoiceManager { get; set; }
        protected IAttachmentManager AttachmentManager { get; set; }
        protected DateHelper DateHelper { get; set; }

        public InvoiceController(UserManager<ApplicationUser> userManager, IInvoiceManager invoiceManager, DateHelper dateHelper, IAttachmentManager attachmentManager)
        {
            UserManager = userManager;
            InvoiceManager = invoiceManager;
            DateHelper = dateHelper;
            AttachmentManager = attachmentManager;
        }

        public IActionResult Index()
        {
            try
            {
                var invoiceList = InvoiceManager.GetInvoiceViewModels();

                return View(invoiceList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = InvoiceManager.GetDefaultInvoiceViewModelForAdd("PLN");
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var invoiceId = InvoiceManager.Add(model, CurrentUser);

                    return RedirectToAction("AddLine", "Invoice", new { invoiceId });
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message + " " + e.InnerException.Message);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult AddJson(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.Add(model, CurrentUser);

                    return Json(new
                    {
                        success = true,
                        Invoice = result
                    });
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";

                ModelState.AddModelError("", e.Message + innerMessage);
            }

            //model.InvoiceLine = new InvoiceLineViewModel();
            //model.BudgetItemList = InvoiceManager.GetOpenBudgetsItemList(model.Budget);
            //model.CustomerItemList = InvoiceManager.GetOpenCustomersItemList(model.Customer);
            //return PartialView("_AddInvoiceForm", model);
            return Json(new
            {
                success = false,
                errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                    .Select(m => m.ErrorMessage).ToArray()
            });
        }

        [HttpGet]
        public IActionResult AddLine(string invoiceId)
        {
            try
            {
                var model = InvoiceManager.GetInvoiceViewModelById(invoiceId);
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult AddLine(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    InvoiceManager.AddLine(model.InvoiceLine, CurrentUser, false, true);

                    return RedirectToAction("AddLine", new { model.InvoiceLine.InvoiceId });
                }

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            var line = model.InvoiceLine;

            model = InvoiceManager.GetInvoiceViewModelById(line.InvoiceId);
            model.InvoiceLine = line;

            return View(model);
        }

        [HttpPost]
        public IActionResult AddLineJson(InvoiceLineViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.AddLine(model, CurrentUser, false, true);

                    return Json(new
                    {
                        success = true,
                        InvoiceLine = result
                    });
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

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var invoice = InvoiceManager.GetInvoiceViewModelById(id);
            invoice.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(id);
            invoice.InvoiceLine = new InvoiceLineViewModel();

            invoice.BudgetItemList = InvoiceManager.GetOpenBudgetsItemList(invoice.Budget);
            invoice.CustomerItemList = InvoiceManager.GetOpenCustomersItemList(invoice.Customer);

            invoice.Attachments = AttachmentManager.GetAttachmentViewModelsForDocument(DocumentType.Invoice, id);

            return View(invoice);
        }

        [HttpPost]
        public IActionResult EditJson(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.Edit(model, CurrentUser);

                    return Json(new
                    {
                        success = true,
                        Invoice = result
                    });
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
        public IActionResult EditLineJson(InvoiceLineViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.EditLine(model, CurrentUser, false, true);

                    return Json(new
                    {
                        success = true,
                        InvoiceLine = result
                    });
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
        public IActionResult CancelLineJson(string lineId, string invoiceId)
        {
            try
            {
                InvoiceManager.ChangeLineStatus(lineId, invoiceId, Status.Cancelled, CurrentUser);

                return Json(new
                {
                    success = true,
                    newStatus = Status.Cancelled.ToString()
                });
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";

                return Json(new
                {
                    success = false,
                    errors = e.Message + innerMessage
                });
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var invoice = InvoiceManager.GetInvoiceViewModelById(id);
            invoice.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(id);
            invoice.Attachments = AttachmentManager.GetAttachmentViewModelsForDocument(DocumentType.Invoice, id);

            return View(invoice);
        }
    }
}
