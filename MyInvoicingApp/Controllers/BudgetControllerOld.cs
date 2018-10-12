using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyInvoicingApp.Controllers
{
    [Authorize]
    public class BudgetControllerOld : Controller
    {
        protected EFCDbContext Context { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected DateHelper DateHelper { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);

        public BudgetControllerOld(EFCDbContext context, UserManager<ApplicationUser> userManager, DateHelper dateHelper)
        {
            Context = context;
            UserManager = userManager;
            DateHelper = dateHelper;
            //CurrentUser = userManager.Users.First(x => x.UserName == User.Identity.Name);
        }

        public IActionResult Index()
        {
            var budgetList = Context.Budgets.Include(x => x.CreatedBy).Include(x => x.Owner)
                .Include(x => x.LastModifiedBy).Select(x => new BudgetViewModel()
                {
                    Id = x.Id,
                    BudgetNumber = x.BudgetNumber,
                    Description = x.Description,
                    CommitedAmount = x.CommitedAmount,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    LastModifiedBy = x.LastModifiedBy,
                    LastModifiedDate = x.LastModifiedDate,
                    Owner = x.Owner
                }).ToList();

            //Select(x => new BudgetViewModelAdapter(x)).ToList();

            //return View(budgetList);
            return Ok();
        }

        [HttpGet]
        public IActionResult Add()
        {
            //return View();
            return Ok();
        }

        [HttpPost]
        public IActionResult Add(BudgetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var newBudget = new BudgetAdapter(model, currentUser, null, null);

                    var newBudget = new Budget()
                    {
                        BudgetNumber = model.BudgetNumber,
                        Description = model.Description,
                        CommitedAmount = model.CommitedAmount,
                        CreatedById = CurrentUser.Id,
                        OwnerId = CurrentUser.Id,
                        CreatedDate = DateHelper.GetCurrentDatetime()
                    };

                    Context.Budgets.Add(newBudget);
                    Context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                throw;
            }

            //return View(model);
            return Ok();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                var model = GetBudgetViewModelById(id);

                //return View(model);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult Edit(BudgetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var budget = GetBudgetById(model.Id);

                    if (model.CommitedAmount != budget.CommitedAmount)
                    {
                        if (GetBudgetInvoiceAmount(budget.Id) <= model.CommitedAmount)
                        {
                            budget.CommitedAmount = model.CommitedAmount;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Wprowadzona nowa wartość budżetu nie pokrywa wartości wszystkich faktur przypisanych do budżetu lub jest ujemna");
                            //return View(model);
                            return Ok();
                        }
                    }

                    budget.Description = model.Description;
                    budget.LastModifiedById = CurrentUser.Id;
                    budget.LastModifiedDate = DateHelper.GetCurrentDatetime();
                    budget.CommitedAmount = model.CommitedAmount;

                    Context.Budgets.Update(budget);
                    Context.SaveChanges();

                    return RedirectToAction("Index");
                }

                //return View(model);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private BudgetViewModel GetBudgetViewModelById(string id)
        {
            var budget = Context.Budgets.Include(x => x.CreatedBy).Include(x => x.Owner).Include(x => x.LastModifiedBy).FirstOrDefault(x => x.Id == id);

            if (budget == null)
            {
                throw new ArgumentException("Bark budżetu o podanym Id");
            }

            var model = new BudgetViewModel()
            {
                Id = budget.Id,
                BudgetNumber = budget.BudgetNumber,
                Description = budget.Description,
                CommitedAmount = budget.CommitedAmount,
                CreatedBy = budget.CreatedBy,
                CreatedDate = budget.CreatedDate,
                LastModifiedBy = budget.LastModifiedBy,
                LastModifiedDate = budget.LastModifiedDate,
                Owner = budget.Owner
            };

            return model;
        }

        private Budget GetBudgetById(string id)
        {
            var budget = Context.Budgets.Include(x => x.InvoiceLines).FirstOrDefault(x => x.Id == id);
            
            if (budget == null)
            {
                throw new ArgumentException("Brak budżetu o podanym Id");
            }

            //Context.Entry(budget).State = EntityState.Detached;

            return budget;
        }

        private decimal GetBudgetInvoiceAmount(string id)
        {
            var amount = Context.InvoiceLines.Where(x => x.BudgetId == id).Sum(x => x.BaseNetto);
            return amount;
        }
    }
}

