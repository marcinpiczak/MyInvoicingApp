using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class BudgetManager : IManager, IBudgetManager
    {
        protected EFCDbContext Context { get; set; }
        protected IDocumentNumberingManager DocumentNumberingManager { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected IDateHelper DateHelper { get; set; }
        protected IDataAccessManager DataAccessManager { get; set; }

        public BudgetManager(EFCDbContext context, UserManager<ApplicationUser> userManager, IDateHelper dateHelper, IDocumentNumberingManager documentNumberingManager, IDataAccessManager dataAccessManager)
        {
            Context = context;
            DocumentNumberingManager = documentNumberingManager;
            UserManager = userManager;
            DateHelper = dateHelper;
            //DocumentAccessManager = documentAccessManager;
            DataAccessManager = dataAccessManager;
        }

        /// <summary>
        /// Gets list of Budgets models
        /// </summary>
        /// <returns>collection with Budget models</returns>
        public IEnumerable<Budget> GetBudgets(IncludeLevel includeLevel)
        {
            IEnumerable<Budget> budgets;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    budgets = Context.Budgets;
                    break;

                case IncludeLevel.Level1:
                    budgets = Context.Budgets
                        .Include(x => x.CreatedBy)
                        .Include(x => x.Owner)
                        .Include(x => x.LastModifiedBy);
                    break;

                default:
                    budgets = Context.Budgets
                        .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                        .Include(x => x.Owner).ThenInclude(x => x.Manager)
                        .Include(x => x.LastModifiedBy);
                    break;
            }

            return budgets;
        }

        /// <summary>
        /// Gets list of BudgetViewModel
        /// </summary>
        /// <returns>collection with BudgetViewModels</returns>
        public IEnumerable<BudgetViewModel> GetBudgetViewModels()
        {
            var models = GetBudgets(IncludeLevel.Level2)
                .Select(x => new BudgetViewModel(x));

            return models;
        }

        public IEnumerable<BudgetViewModel> GetBudgetViewModelsForUser(ApplicationUser user)
        {
            var models = GetBudgetViewModels()
                .Where(x => DataAccessManager.CanView(x, user))
                .Select(x => DataAccessManager.GetBudgetViewModelAccess(x, user));

            return models;
        }

        /// <summary>
        /// Gets Budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="budgetId">Budget id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        public Budget GetBudgetById(string budgetId, IncludeLevel includeLevel)
        {

            Budget budget;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    budget = Context.Budgets
                        .FirstOrDefault(x => x.Id == budgetId);
                    break;

                case IncludeLevel.Level1:
                    budget = Context.Budgets
                        .Include(x => x.CreatedBy)
                        .Include(x => x.Owner)
                        .Include(x => x.LastModifiedBy)
                        .FirstOrDefault(x => x.Id == budgetId);
                    break;

                default:
                    budget = Context.Budgets
                        .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                        .Include(x => x.Owner).ThenInclude(x => x.Manager)
                        .Include(x => x.LastModifiedBy)
                        .FirstOrDefault(x => x.Id == budgetId);
                    break;
            }

            if (budget == null)
            {
                throw new ArgumentException("Brak budżetu o podanym Id", nameof(budget));
            }

            return budget;
        }

        /// <summary>
        /// Gets BudgetViewModel based on budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="budgetId">Budget id</param>
        /// <returns>BudgetViewModel based on budget for given Id</returns>
        public BudgetViewModel GetBudgetViewModelById(string budgetId)
        {
            var budget = GetBudgetById(budgetId, IncludeLevel.Level2);

            var model = new BudgetViewModel(budget);

            return model;
        }

        public BudgetViewModel GetBudgetViewModelByIdForUser(string budgetId, ApplicationUser user)
        {
            var model = GetBudgetViewModelById(budgetId);

            if (!DataAccessManager.CanView(model, user))
            {
                throw new InvalidOperationException("Nie masz uprawnień do przeglądania tego budżetu");
            }

            model = DataAccessManager.GetBudgetViewModelAccess(model, user);

            return model;
        }

        /// <summary>
        /// Add Budget from given BudgetViewModel.
        /// </summary>
        /// <param name="model">BudgetViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        public BudgetReturnResult Add(BudgetViewModel model, ApplicationUser createdBy)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(BudgetViewModel), "Nieprawidłowe parametry");
            }

            var now = DateHelper.GetCurrentDatetime();
            var documentNumber = DocumentNumberingManager.GetNextDocumentNumber(DocumentType.Budget, now, createdBy);

            var budget = Context.Budgets.SingleOrDefault(x => x.BudgetNumber == documentNumber);

            if (budget != null)
            {
                throw new ArgumentException($"Budżet o podanym numerze {documentNumber} już istnieje. Spróbuj ponownie a jeżeli problem będzie się powtarzał przestaw ręcznie numerację", nameof(Budget));
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

            return new BudgetReturnResult()
            {
                Id = newBudget.Id,
                BudgetNumber = newBudget.BudgetNumber,
                Status = newBudget.Status.ToString()
            };
        }

        /// <summary>
        /// Modify Budget from given BudgetViewModel or throws exceptions if budget not found.
        /// </summary>
        /// <param name="model">BudgetViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        public BudgetReturnResult Edit(BudgetViewModel model, ApplicationUser modifiedBy)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException("model", "Nieprawidłowe parametry");
            }

            var budget = GetBudgetById(model.Id, IncludeLevel.Level2);

            if (!DataAccessManager.CanEdit(new BudgetViewModel(budget), modifiedBy))
            {
                throw new InvalidOperationException("Nie masz uprawnień do edycji tego budżetu");
            }

            if (model.CommitedAmount != budget.CommitedAmount)
            {
                if (GetBudgetBaseNettoTotalInvoicesAmount(budget.Id) <= model.CommitedAmount)
                {
                    budget.CommitedAmount = model.CommitedAmount;
                }
                else
                {
                    throw new Exception("Wprowadzona nowa wartość budżetu " + (model.CommitedAmount < 0 ? "jest ujemna" : "nie pokrywa wartości wszystkich faktur przypisanych do budżetu"));
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

            return new BudgetReturnResult()
            {
                Id = budget.Id,
                BudgetNumber = budget.BudgetNumber,
                Status = budget.Status.ToString()
            };
        }

        /// <summary>
        /// Sum base netto from all invoices asigned to given budget
        /// </summary>
        /// <param name="budgetId">budget id for which totaling of invoice base netto amount should be done</param>
        /// <returns>total base netto from all invoices asigned to budget</returns>
        public decimal GetBudgetBaseNettoTotalInvoicesAmount(string budgetId)
        {
            var amount = Context.InvoiceLines
                .Where(x => x.BudgetId == budgetId)
                .Where(x => x.Status != Status.Cancelled)
                .Sum(x => x.BaseNetto);

            return amount;
        }

        /// <summary>
        /// Changes status for given budget
        /// </summary>
        /// <param name="budgetId">budget id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        public BudgetReturnResult ChangeStatus(string budgetId, Status newStatus, ApplicationUser modifiedBy)
        {
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy), "Nieprawidłowe parametry");
            }

            var budget = GetBudgetById(budgetId, IncludeLevel.None);

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

            return new BudgetReturnResult()
            {
                Id = budget.Id,
                BudgetNumber = budget.BudgetNumber,
                Status = budget.Status.ToString()
            };
        }

        public BudgetReturnResult Close(string budgetId, ApplicationUser modifiedBy)
        {
            var model = GetBudgetViewModelById(budgetId);

            if (!DataAccessManager.CanClose(model, modifiedBy))
            {
                throw new InvalidOperationException("Nie masz uprawnień do zamykania tego budżetu");
            }

            return ChangeStatus(budgetId, Status.Closed, modifiedBy);
        }

        public BudgetReturnResult Open(string budgetId, ApplicationUser modifiedBy)
        {
            var model = GetBudgetViewModelById(budgetId);

            if (!DataAccessManager.CanOpen(model, modifiedBy))
            {
                throw new InvalidOperationException("Nie masz uprawnień do otwierania tego budżetu");
            }

            return ChangeStatus(budgetId, Status.Opened, modifiedBy);
        }

        /// <summary>
        /// Updates budget invoiced amount with given amount and/or recalculate invoiced amount with total of base netto amount from invoices asigned to budget
        /// </summary>
        /// <param name="budgetId">budget id</param>
        /// <param name="invoiceAmount">invoice amount that need to update budgets invoiced amount</param>
        /// <param name="recalculateInvoicedAmount">indicates if recalculation of budget invoiced amount should to be done</param>
        public void UpdateBudgetInvoicedAmount(string budgetId, decimal invoiceAmount, bool recalculateInvoicedAmount)
        {
            var budget = GetBudgetById(budgetId, IncludeLevel.None);

            var currentInvoicedAmount = budget.InvoicedAmount;

            if (recalculateInvoicedAmount)
            {
                currentInvoicedAmount = GetBudgetBaseNettoTotalInvoicesAmount(budgetId);
            }

            if (currentInvoicedAmount + invoiceAmount > budget.CommitedAmount)
            {
                throw new InvalidOperationException($"Alokacja kwoty faktury spodouje przekroczenie wartości budżetu. " +
                                                    $"Wartość faktury: {invoiceAmount}, " +
                                                    $"wartość dotychczas zaalokowanych faktur: {budget.InvoicedAmount}, " +
                                                    $"wartość budżetu: {budget.CommitedAmount}");
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
    }
}