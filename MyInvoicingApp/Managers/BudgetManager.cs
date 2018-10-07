using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class BudgetManager : IManager, IBudgetManager
    {
        protected EFCDbContext Context { get; set; }
        protected IDocumentNumberingManager DocumentNumberingManager { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected DateHelper DateHelper { get; set; }

        public BudgetManager(EFCDbContext context, UserManager<ApplicationUser> userManager, DateHelper dateHelper, IDocumentNumberingManager documentNumberingManager)
        {
            Context = context;
            DocumentNumberingManager = documentNumberingManager;
            UserManager = userManager;
            DateHelper = dateHelper;
        }

        public IEnumerable<Budget> GetBudgets()
        {
            var budgets = Context.Budgets
                .Include(x => x.CreatedBy)
                .Include(x => x.Owner)
                .Include(x => x.LastModifiedBy);

            return budgets;
        }

        public IEnumerable<BudgetViewModel> GetBudgetViewModels()
        {
            var models = GetBudgets()
                //.Select(x => new BudgetViewModel()
                //{
                //    Id = x.Id,
                //    Status = x.Status,
                //    BudgetNumber = x.BudgetNumber,
                //    Description = x.Description,
                //    CommitedAmount = x.CommitedAmount,
                //    InvoicedAmount = x.InvoicedAmount,
                //    CreatedBy = x.CreatedBy,
                //    CreatedDate = x.CreatedDate,
                //    LastModifiedBy = x.LastModifiedBy,
                //    LastModifiedDate = x.LastModifiedDate,
                //    Owner = x.Owner
                //});
                .Select(x => new BudgetViewModel(x));

            return models;
        }

        public Budget GetBudgetById(string budgetId)
        {
            var budget = Context.Budgets
                .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                .Include(x => x.Owner)
                .Include(x => x.LastModifiedBy)
                .FirstOrDefault(x => x.Id == budgetId);

            if (budget == null)
            {
                throw new ArgumentException("Brak budżetu o podanym Id", nameof(budget));
            }

            //Context.Entry(budget).State = EntityState.Detached;

            return budget;
        }

        public Budget GetBudgetByIdSimple(string budgetId)
        {
            var budget = Context.Budgets
                .FirstOrDefault(x => x.Id == budgetId);

            if (budget == null)
            {
                throw new ArgumentException("Brak budżetu o podanym Id", nameof(budget));
            }

            return budget;
        }

        public BudgetViewModel GetBudgetViewModelById(string budgetId)
        {
            var budget = GetBudgetById(budgetId);

            //var model = new BudgetViewModel()
            //{
            //    Id = budget.Id,
            //    Status = budget.Status,
            //    BudgetNumber = budget.BudgetNumber,
            //    Description = budget.Description,
            //    CommitedAmount = budget.CommitedAmount,
            //    InvoicedAmount = budget.InvoicedAmount,
            //    CreatedBy = budget.CreatedBy,
            //    CreatedDate = budget.CreatedDate,
            //    LastModifiedBy = budget.LastModifiedBy,
            //    LastModifiedDate = budget.LastModifiedDate,
            //    Owner = budget.Owner
            //};

            var model = new BudgetViewModel(budget);

            return model;
        }

        public void Add(BudgetViewModel model, ApplicationUser createdBy)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var now = DateHelper.GetCurrentDatetime();
            var documentNumber = DocumentNumberingManager.GetNextDocumentNumber(DocumentType.Budget, now, createdBy);

            var budget = Context.Budgets.SingleOrDefault(x => x.BudgetNumber == documentNumber);

            if (budget != null)
            {
                throw new ArgumentException($"Budżet o podanym numerze {documentNumber} już istnieje. Spróbuj ponownie a jeżeli problem będzie się powtarzał przestaw ręcznie numerację", nameof(budget));
            }

            var newBudget = new Budget()
            {
                Status = Status.Opened,
                BudgetNumber = documentNumber,
                Description = model.Description,
                CommitedAmount = model.CommitedAmount,
                InvoicedAmount = 0M,
                CreatedById = createdBy.Id,
                OwnerId = createdBy.Id,
                CreatedDate = now
            };

            Context.Budgets.Add(newBudget);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }

        public void Edit(BudgetViewModel model, ApplicationUser modifiedBy)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException("model", "Nieprawidłowe parametry");
            }

            var budget = GetBudgetById(model.Id);

            if (!CanEdit(model.CreatedBy, modifiedBy))
            {
                throw new InvalidOperationException("Nie możesz edytować czyjegoś Budżetu");
            }

            if (model.CommitedAmount != budget.CommitedAmount)
            {
                if (GetBudgetBaseNettoTotalInvoicesAmount(budget.Id) <= model.CommitedAmount)
                {
                    budget.CommitedAmount = model.CommitedAmount;
                }
                else
                {
                    throw new Exception("Wprowadzona nowa wartość budżetu" + (model.CommitedAmount < 0 ? "jest ujemna" : "nie pokrywa wartości wszystkich faktur przypisanych do budżetu"));
                }
            }

            var now = DateHelper.GetCurrentDatetime();

            budget.Description = model.Description;
            budget.LastModifiedById = modifiedBy.Id;
            budget.LastModifiedDate = now;
            budget.CommitedAmount = model.CommitedAmount;

            Context.Budgets.Update(budget);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }

        public decimal GetBudgetBaseNettoTotalInvoicesAmount(string budgetId)
        {
            var amount = Context.InvoiceLines
                .Where(x => x.BudgetId == budgetId)
                .Where(x => x.Status != Status.Cancelled)
                .Sum(x => x.BaseNetto);

            return amount;
        }

        public IEnumerable<InvoiceLine> GetInvoiceLinesForBudget(string budgetId)
        {
            var invoicesList = Context.InvoiceLines
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .Include(x => x.Invoice).ThenInclude(x => x.Customer)
                .Include(x => x.Budget)
                .Where(x => x.BudgetId == budgetId);

            return invoicesList;
        }

        public IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModelsForBudget(string budgetId)
        {
            var models = GetInvoiceLinesForBudget(budgetId)
                //    .Select(x => new InvoiceLineViewModel()
                //    {
                //        Id = x.Id,
                //        InvoiceId = x.InvoiceId,
                //        Status = x.Status,
                //        CreatedBy = x.CreatedBy,
                //        CreatedDate = x.CreatedDate,
                //        LastModifiedBy = x.LastModifiedBy,
                //        LastModifiedDate = x.LastModifiedDate,
                //        Invoice = x.Invoice,
                //        LineNumber = x.LineNumber,
                //        ItemName = x.ItemName,
                //        Description = x.Description,
                //        Quantity = x.Quantity,
                //        Price = x.Price,
                //        Currency = x.Currency,
                //        CurrencyRate = x.CurrencyRate,
                //        TaxRate = x.TaxRate,
                //        Netto = x.Netto,
                //        Tax = x.Tax,
                //        Gross = x.Gross,
                //        BaseNetto = x.BaseNetto,
                //        BaseTax = x.BaseTax,
                //        BaseGross = x.BaseGross,
                //        Budget = x.Budget
                //    });
                .Select(x => new InvoiceLineViewModel(x));

            return models;
        }

        public void ChangeStatus(string budgetId, Status newStatus, ApplicationUser modifiedBy)
        {
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy), "Nieprawidłowe parametry");
            }

            var budget = GetBudgetById(budgetId);

            if (newStatus == Status.Closed)
            {
                UpdateBudgetInvoicedAmount(budgetId, 0M, true);
                budget.CommitedAmount = GetBudgetBaseNettoTotalInvoicesAmount(budgetId);
            }

            var now = DateHelper.GetCurrentDatetime();

            budget.Status = newStatus;
            budget.LastModifiedById = modifiedBy.Id;
            budget.LastModifiedDate = now;

            Context.Budgets.Update(budget);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }

        public void UpdateBudgetInvoicedAmount(string budgetId, decimal invoiceAmount, bool recalculateInvoicedAmount)
        {
            var budget = GetBudgetById(budgetId);

            var currentInvoicedAmount = budget.InvoicedAmount;

            if (recalculateInvoicedAmount)
            {
                //sprawdzić kwestię jednoczesnego dostępu
                currentInvoicedAmount = GetBudgetBaseNettoTotalInvoicesAmount(budgetId);
            }

            if (currentInvoicedAmount + invoiceAmount > budget.CommitedAmount)
            {
                throw new InvalidOperationException($"Alokacja kwoty faktury spodouje przekroczenie wartości budżetu. " +
                                                    $"Wartość faktury: {invoiceAmount}, " +
                                                    $"wartość dotychczas zaalokowanych faktur {budget.InvoicedAmount}, " +
                                                    $"wartość budżetu {budget.CommitedAmount}");
            }

            currentInvoicedAmount += invoiceAmount;

            budget.InvoicedAmount = currentInvoicedAmount;

            Context.Budgets.Update(budget);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }

        public bool CanEdit(ApplicationUser createdBy, ApplicationUser modifiedBy)
        {
            if (createdBy.UserName == modifiedBy.UserName || createdBy.Manager.UserName == modifiedBy.UserName)
            {
                return true;
            }

            return false;
        }
    }
}