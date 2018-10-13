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
    [Authorize]
    public class BudgetController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected IBudgetManager BudgetManager { get; set; }

        public BudgetController(UserManager<ApplicationUser> userManager, IBudgetManager budgetManager)
        {
            UserManager = userManager;
            BudgetManager = budgetManager;
        }

        public IActionResult Index()
        {
            try
            {
                var budgetViewModels = BudgetManager.GetBudgetViewModels().OrderByDescending(x => x.CreatedDate);

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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(BudgetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = BudgetManager.Add(model, CurrentUser);

                    TempData["Success"] = $"Dodano nowy Budżet z numerem {result.BudgetNumber}";
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
        public IActionResult AddJson(BudgetViewModel model)
        {
            try
            {
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
        public IActionResult Edit(string id)
        {
            try
            {
                var model = BudgetManager.GetBudgetViewModelById(id);

                if (!BudgetManager.CanEdit(model.CreatedBy, CurrentUser))
                {
                    throw new InvalidOperationException("Nie możesz edytować czyjegoś Budżetu");
                }

                return View(model);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas edytowania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(BudgetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BudgetManager.Edit(model, CurrentUser);

                    TempData["Success"] = $"Zapisano wprowadzone zmiany w Budżecie {model.BudgetNumber}";
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
        public IActionResult Close(string id)
        {
            try
            {
                var result = BudgetManager.ChangeStatus(id, Status.Closed, CurrentUser);
                TempData["Success"] = $"Budżet z numerem {result.BudgetNumber} został zamknięty";
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
        public IActionResult ReOpen(string id)
        {
            try
            {
                var result = BudgetManager.ChangeStatus(id, Status.Opened, CurrentUser);
                TempData["Success"] = $"Budżet z numerem {result.BudgetNumber} został otwarty";
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
        public IActionResult Details(string id)
        {
            try
            {
                var model = BudgetManager.GetBudgetViewModelById(id);

                model.InvoiceLines = BudgetManager.GetInvoiceLineViewModelsForBudget(id).ToList();

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

