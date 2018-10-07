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
                var budgetViewModels = BudgetManager.GetBudgetViewModels();

                return View(budgetViewModels);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public IActionResult IndexPartial()
        {
            try
            {
                //var budgetViewModels = BudgetManager.GetBudgetViewModels();

                var budgetViewModels = new List<BudgetViewModel>()
                {
                    new BudgetViewModel()
                };

                return PartialView("_index", budgetViewModels);
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
            return View();
        }

        [HttpGet]
        public IActionResult AddPartial()
        {
            return PartialView("_add");
        }

        [HttpPost]
        public IActionResult Add(BudgetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BudgetManager.Add(model, CurrentUser);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                //throw;
            }

            return View(model);
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
                TempData["Message"] = e.Message;
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

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                //throw;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Close(string id)
        {
            try
            {
                BudgetManager.ChangeStatus(id, Status.Closed, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ReOpen(string id)
        {
            try
            {
                BudgetManager.ChangeStatus(id, Status.Opened, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
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
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}

