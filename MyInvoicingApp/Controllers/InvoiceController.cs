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
    [Authorize(Roles = "Admin,Accountant,Manager")]
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
                var invoiceList = InvoiceManager.GetInvoiceViewModels().OrderByDescending(x => x.CreatedDate);

                return View(invoiceList);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy faktur: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            try
            {
                var model = InvoiceManager.GetDefaultInvoiceViewModelForAdd("PLN");
            
                return View(model);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas pobierania domyslnych wartości dla faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Add(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.Add(model, CurrentUser);

                    TempData["Success"] = $"Dodano nową fakturę z numerem <b>{result.InvoiceNumber}</b>";

                    return RedirectToAction("AddLine", "Invoice", new { result.Id });
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas dodawania nowej faktury: {e.Message}{innerMessage}";
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
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas dodwania nowej linii do faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddLine(InvoiceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.AddLine(model.InvoiceLine, CurrentUser, false, true);

                    TempData["Success"] = $"Dodano nową linię do faktury <b>{model.InvoiceNumber}</b>";

                    return RedirectToAction("AddLine", new { model.InvoiceLine.InvoiceId });
                }

                var line = model.InvoiceLine;

                model = InvoiceManager.GetInvoiceViewModelById(line.InvoiceId);
                model.InvoiceLine = line;

                return View(model);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas dodwania nowej linii do faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
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
            try
            {
                var invoice = InvoiceManager.GetInvoiceViewModelById(id);
                invoice.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(id);
                invoice.InvoiceLine = new InvoiceLineViewModel();

                invoice.BudgetItemList = InvoiceManager.GetOpenBudgetsItemList(invoice.Budget);
                invoice.CustomerItemList = InvoiceManager.GetOpenCustomersItemList(invoice.Customer);

                invoice.Attachments = AttachmentManager.GetAttachmentViewModelsForDocument(DocumentType.Invoice, id);

                return View(invoice);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas edytowania faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
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
            try
            {
                var invoice = InvoiceManager.GetInvoiceViewModelById(id);
                invoice.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(id);
                invoice.Attachments = AttachmentManager.GetAttachmentViewModelsForDocument(DocumentType.Invoice, id);

                return View(invoice);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświelania szczegółów faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Cancel(string invoiceId)
        {
            try
            {
                var result = InvoiceManager.ChangeStatus(invoiceId, Status.Cancelled, CurrentUser);

                TempData["Success"] = $"Faktura <b>{result.InvoiceNumber}</b> została anulowana";
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                TempData["Error"] = $"Wystąpił problem podczas anulowania faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }
    }
}
