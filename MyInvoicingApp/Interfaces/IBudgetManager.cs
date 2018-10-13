using System.Collections.Generic;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IBudgetManager
    {
        /// <summary>
        /// Returns IEnumberable with Budgets
        /// </summary>
        /// <returns>IEnumberable of type Budget</returns>
        IEnumerable<BudgetViewModel> GetBudgetViewModels();

        IEnumerable<Budget> GetBudgets();

        /// <summary>
        /// Returns BudgetViewModel based on budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="id">Budget id</param>
        /// <returns>BudgetViewModel based on budget for given Id</returns>
        BudgetViewModel GetBudgetViewModelById(string id);

        /// <summary>
        /// Returns Budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="id">Budget id</param>
        /// <returns>Budget based on budget for given Id</returns>
        Budget GetBudgetById(string id);

        Budget GetBudgetByIdSimple(string budgetId);

        /// <summary>
        /// Add Budget from given BudgetViewModel or throws exceptions if budget not found.
        /// </summary>
        /// <param name="model">BudgetViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Budget</param>
        BudgetReturnResult Add(BudgetViewModel model, ApplicationUser createdBy);

        BudgetReturnResult Edit(BudgetViewModel model, ApplicationUser modifiedBy);

        decimal GetBudgetBaseNettoTotalInvoicesAmount(string id);

        IEnumerable<InvoiceLine> GetInvoiceLinesForBudget(string id);

        IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModelsForBudget(string id);

        BudgetReturnResult ChangeStatus(string id, Status newStatus, ApplicationUser modifiedBy);

        void UpdateBudgetInvoicedAmount(string budgetId, decimal invoiceAmount, bool recalculateInvoicedAmount);

        bool CanEdit(ApplicationUser createdBy, ApplicationUser modifiedBy);
    }
}