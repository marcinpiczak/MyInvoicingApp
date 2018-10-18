using System.Collections.Generic;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IBudgetManager
    {
        /// <summary>
        /// Gets list of Budgets models
        /// </summary>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with Budget models</returns>
        IEnumerable<Budget> GetBudgets(IncludeLevel includeLevel);

        /// <summary>
        /// Gets list of BudgetViewModels
        /// </summary>
        /// <returns>collection with BudgetViewModels</returns>
        IEnumerable<BudgetViewModel> GetBudgetViewModels();

        /// <summary>
        /// Gets Budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="budgetId">Budget id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        Budget GetBudgetById(string budgetId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets BudgetViewModel based on budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="budgetId">Budget id</param>
        /// <returns>BudgetViewModel based on budget for given Id</returns>
        BudgetViewModel GetBudgetViewModelById(string budgetId);

        /// <summary>
        /// Add Budget from given BudgetViewModel.
        /// </summary>
        /// <param name="model">BudgetViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        BudgetReturnResult Add(BudgetViewModel model, ApplicationUser createdBy);

        /// <summary>
        /// Modify Budget from given BudgetViewModel or throws exceptions if budget not found.
        /// </summary>
        /// <param name="model">BudgetViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        BudgetReturnResult Edit(BudgetViewModel model, ApplicationUser modifiedBy);

        /// <summary>
        /// Sum base netto from all invoices asigned to given budget
        /// </summary>
        /// <param name="budgetId">budget id for which totaling of invoice base netto amount should be done</param>
        /// <returns>total base netto from all invoices asigned to budget</returns>
        decimal GetBudgetBaseNettoTotalInvoicesAmount(string budgetId);

        /// <summary>
        /// Changes status for given budget
        /// </summary>
        /// <param name="budgetId">budget id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying budget</param>
        /// <returns>BudgetReturnResult with id, budget number and status</returns>
        BudgetReturnResult ChangeStatus(string budgetId, Status newStatus, ApplicationUser modifiedBy);

        /// <summary>
        /// Updates budget invoiced amount with given amount and/or recalculate invoiced amount with total of base netto amount from invoices asigned to budget
        /// </summary>
        /// <param name="budgetId">budget id</param>
        /// <param name="invoiceAmount">invoice amount that need to update budgets invoiced amount</param>
        /// <param name="recalculateInvoicedAmount">indicates if recalculation of budget invoiced amount should to be done</param>
        void UpdateBudgetInvoicedAmount(string budgetId, decimal invoiceAmount, bool recalculateInvoicedAmount);

        /// <summary>
        /// Simple checking if budget can be modified by user
        /// </summary>
        /// <param name="createdBy">ApplicationUser that created Budget</param>
        /// <param name="modifiedBy">ApplicationUser that modifies Budget</param>
        /// <returns></returns>
        bool CanEdit(ApplicationUser createdBy, ApplicationUser modifiedBy);
    }
}