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
    public class CustomerController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected ICustomerManager CustomerManager { get; set; }
        protected IInvoiceManager InvoiceManager { get; set; }
        protected IModuleAccessManager ModuleAccessManager { get; set; }

        public CustomerController(UserManager<ApplicationUser> userManager, ICustomerManager customerManager, IInvoiceManager invoiceManager, IModuleAccessManager moduleAccessManager)
        {
            UserManager = userManager;
            CustomerManager = customerManager;
            InvoiceManager = invoiceManager;
            ModuleAccessManager = moduleAccessManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Index, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Lista w module Klienci. Skontaktuj się z administratorem");
                }

                var customerViewModels = CustomerManager.GetCustomerViewModelsForUser(CurrentUser)
                    .OrderByDescending(x => x.CreatedDate);

                return View(customerViewModels);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy klientów: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Klienci. Skontaktuj się z administratorem");
                }

                return View();
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy klientów: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CustomerViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Add, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Dodawania w module Klienci. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    var result = CustomerManager.Add(model, CurrentUser);

                    TempData["Success"] = $"Dodano nowego klienta <b>{result.Name}</b>";
                    //return RedirectToAction("Index");
                    return RedirectToAction("Add");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas dodawania nowego klienta: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Klienci. Skontaktuj się z administratorem");
                }

                var model = CustomerManager.GetCustomerViewModelByIdForUser(id, CurrentUser);

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
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Edit, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Edycji w module Klienci. Skontaktuj się z administratorem");
                }

                if (ModelState.IsValid)
                {
                    var result = CustomerManager.Edit(model, CurrentUser);

                    TempData["Success"] = $"Zapisano wprowadzone zmiany dla klienta <b>{result.Name}</b>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas zapisywania zmian dla klienta: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Close(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Close, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Zamykania w module Klienci. Skontaktuj się z administratorem");
                }

                //var result = CustomerManager.ChangeStatus(id, Status.Closed, CurrentUser);
                var result = CustomerManager.Close(id, CurrentUser);
                TempData["Success"] = $"Klient <b>{result.Name}</b> został zamknięty";
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
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Open, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji Otwierania w module Klienci. Skontaktuj się z administratorem");
                }

                //var result = CustomerManager.ChangeStatus(id, Status.Opened, CurrentUser);
                var result = CustomerManager.Open(id, CurrentUser);
                TempData["Success"] = $"Klient <b>{result.Name}</b> został otwarty";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas otwierania klienta: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var access = await ModuleAccessManager.CheckModuleActionAccessAsync(Models.Controllers.Customer, Actions.Details, CurrentUser);

                if (!access)
                {
                    throw new UnauthorizedAccessException("Nie posiadasz uprawnień do akcji wyświetlania Szczegółów w module Klienci. Skontaktuj się z administratorem");
                }

                //var model = CustomerManager.GetCustomerViewModelById(id);
                var model = CustomerManager.GetCustomerViewModelByIdForUser(id, CurrentUser);

                model.Invoices = InvoiceManager.GetInvoiceViewModelsForCustomer(id).ToList();
            
                return View(model);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas wyświelania szczegółów klienta: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }
    }
}
