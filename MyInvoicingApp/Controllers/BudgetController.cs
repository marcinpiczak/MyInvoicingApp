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
    [Authorize]
    public class BudgetController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected IBudgetManager BudgetManager { get; set; }
        protected IInvoiceManager InvoiceManager { get; set; }
        protected IModuleAccessManager ModuleAccessManager { get; set; }

        public BudgetController(UserManager<ApplicationUser> userManager, IBudgetManager budgetManager, IInvoiceManager invoiceManager/*, IDocumentAccessManager documentAccessManager*/, IModuleAccessManager moduleAccessManager)
        {
            UserManager = userManager;
            BudgetManager = budgetManager;
            InvoiceManager = invoiceManager;
            //DocumentAccessManager = documentAccessManager;
            ModuleAccessManager = moduleAccessManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Index, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Lista w module Budżet. Skontaktuj się z administratorem");
                }

                var budgetViewModels = BudgetManager.GetBudgetViewModelsForUser(CurrentUser)
                    .OrderByDescending(x => x.CreatedDate);
                    
                return View(budgetViewModels);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy budżetów: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Budżet. Skontaktuj się z administratorem");
                }

                return View();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy budżetów: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(BudgetViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Budżet. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    var result = BudgetManager.Add(model, CurrentUser);

                    TempData["Success"] = $"Dodano nowy Budżet z numerem <b>{result.BudgetNumber}</b>";
                    //return RedirectToAction("Index");
                    return RedirectToAction("Add");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas dodawania nowego budżetu: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddJson(BudgetViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Budżet. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    var result = BudgetManager.Add(model, CurrentUser);

                    return Json(new
                    {
                        success = true,
                        Budget = result
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
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Budżet. Skontaktuj się z administratorem");
                }

                var model = BudgetManager.GetBudgetViewModelByIdForUser(id, CurrentUser);

                return View(model);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas edytowania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BudgetViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Budżet. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    BudgetManager.Edit(model, CurrentUser);

                    TempData["Success"] = $"Zapisano wprowadzone zmiany w Budżecie <b>{model.BudgetNumber}</b>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas zapisywania zmian do budżetu: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Close(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Close, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Zamykania w module Budżet. Skontaktuj się z administratorem");
                }

                //var result = BudgetManager.ChangeStatus(id, Status.Closed, CurrentUser);
                var result = BudgetManager.Close(id, CurrentUser);
                TempData["Success"] = $"Budżet z numerem <b>{result.BudgetNumber}</b> został zamknięty";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas zamykania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ReOpen(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Open, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Otwierania w module Budżet. Skontaktuj się z administratorem");
                }

                //var result = BudgetManager.ChangeStatus(id, Status.Opened, CurrentUser);
                var result = BudgetManager.Open(id, CurrentUser);
                TempData["Success"] = $"Budżet z numerem <b>{result.BudgetNumber}</b> został otwarty";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas otwierania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Budget, Actions.Details, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji wyświetlania Szczegółów w module Budżet. Skontaktuj się z administratorem");
                }

                //var model = BudgetManager.GetBudgetViewModelById(id);
                var model = BudgetManager.GetBudgetViewModelByIdForUser(id, CurrentUser);

                model.InvoiceLines = InvoiceManager.GetInvoiceLineViewModelsForBudget(id).ToList();

                return View(model);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświelania szczegółów budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }
    }
}

