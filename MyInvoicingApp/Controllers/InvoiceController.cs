using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        protected IDateHelper DateHelper { get; set; }
        protected IModuleAccessManager ModuleAccessManager { get; set; }

        public InvoiceController(UserManager<ApplicationUser> userManager, IInvoiceManager invoiceManager, IDateHelper dateHelper, IAttachmentManager attachmentManager, IModuleAccessManager moduleAccessManager)
        {
            UserManager = userManager;
            InvoiceManager = invoiceManager;
            DateHelper = dateHelper;
            AttachmentManager = attachmentManager;
            ModuleAccessManager = moduleAccessManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Index, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Lista w module Faktury. Skontaktuj się z administratorem");
                }

                var invoiceList = InvoiceManager.GetInvoiceViewModelsForUser(CurrentUser)
                    .OrderByDescending(x => x.CreatedDate);

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
        public async Task<IActionResult> Add()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

                var model = InvoiceManager.GetDefaultInvoiceViewModelForAddForUser(CurrentUser, "PLN");
            
                return View(model);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas dodawania nowej faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Add(InvoiceViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> AddJson(InvoiceViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> AddLine(string invoiceId)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

                var model = InvoiceManager.GetInvoiceViewModelByIdForUser(invoiceId, CurrentUser);
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
        public async Task<IActionResult> AddLine(InvoiceViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    var result = InvoiceManager.AddLine(model.InvoiceLine, CurrentUser, false, true);

                    TempData["Success"] = $"Dodano nową linię do faktury <b>{model.InvoiceNumber}</b>";

                    return RedirectToAction("AddLine", new { model.InvoiceLine.InvoiceId });
                }

                var line = model.InvoiceLine;

                model = InvoiceManager.GetInvoiceViewModelByIdForUser(line.InvoiceId, CurrentUser);
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
        public async Task<IActionResult> AddLineJson(InvoiceLineViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> EditJson(InvoiceViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> EditLineJson(InvoiceLineViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Faktury. Skontaktuj się z administratorem");
                }

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
        public async Task<IActionResult> CancelLineJson(string lineId, string invoiceId)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Cancel, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Anulowania w module Faktury. Skontaktuj się z administratorem");
                }

                InvoiceManager.CancelInvoiceLine(lineId, invoiceId, CurrentUser);

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
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Details, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji wyświetlania Szczegółów w module Faktury. Skontaktuj się z administratorem");
                }

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

        public async Task<IActionResult> Cancel(string invoiceId)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Invoice, Actions.Cancel, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Anulowania w module Faktury. Skontaktuj się z administratorem");
                }

                var result = InvoiceManager.CancelInvoice(invoiceId, CurrentUser);

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
