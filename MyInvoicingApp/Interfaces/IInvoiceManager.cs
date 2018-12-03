using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IInvoiceManager
    {
        /// <summary>
        /// Gets collection of Invoice models
        /// </summary>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with Invoice models</returns>
        IEnumerable<Invoice> GetInvoices(IncludeLevel includeLevel);

        /// <summary>
        /// Gets collection of InvoiceViewModels
        /// </summary>
        /// <returns>collection with InvoiceViewModels</returns>
        IEnumerable<InvoiceViewModel> GetInvoiceViewModels();

        IEnumerable<InvoiceViewModel> GetInvoiceViewModelsForUser(ApplicationUser user);

        /// <summary>
        /// Gets Invoice for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        Invoice GetInvoiceById(string invoiceId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets InvoiceViewModel based on invoice for given Id or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <returns>InvoiceViewModel based on invoice for given Id</returns>
        InvoiceViewModel GetInvoiceViewModelById(string invoiceId);

        InvoiceViewModel GetInvoiceViewModelByIdForUser(string invoiceId, ApplicationUser user);

        /// <summary>
        /// Add Invoice based on given InvoiceViewModel.
        /// </summary>
        /// <param name="model">InvoiceViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        InvoiceReturnResult Add(InvoiceViewModel model, ApplicationUser createdBy);

        /// <summary>
        /// Modify Invoice based on given InvoiceViewModel or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="model">InvoiceViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        InvoiceReturnResult Edit(InvoiceViewModel model, ApplicationUser modifiedBy);

        /// <summary>
        /// Changes status for given invoice
        /// </summary>
        /// <param name="invoiceId">invoice id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        InvoiceReturnResult ChangeStatus(string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        InvoiceReturnResult CancelInvoice(string invoiceId, ApplicationUser modifiedBy);

        /// <summary>
        /// Gets InvoiceViewModel with default values set for Add method
        /// </summary>
        /// <param name="defaultCurrency">Default currency</param>
        /// <returns>InvoiceViewModel with default values set</returns>
        InvoiceViewModel GetDefaultInvoiceViewModelForAdd(string defaultCurrency = "PLN");

        InvoiceViewModel GetDefaultInvoiceViewModelForAddForUser(ApplicationUser user, string defaultCurrency = "PLN");

        /// <summary>
        /// Gets collection of Invoice line models for given invoice Id
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with Invoice models</returns>
        IEnumerable<InvoiceLine> GetInvoiceLines(string invoiceId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets collection of InvoiceLineViewModels
        /// </summary>
        /// <returns>collection with InvoiceLineViewModels</returns>
        IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModels(string invoiceId);

        /// <summary>
        /// Gets Invoice line for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="lineId">Invoice line id</param>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        InvoiceLine GetInvoiceLineById(string lineId, string invoiceId, IncludeLevel includeLevel);

        /// <summary>
        /// Add Invoice line based on given InvoiceLineViewModel.
        /// </summary>
        /// <param name="model">InvoiceLineViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates invoice line</param>
        /// <param name="recalculateInvoiceValues">Indicates if invoice line values should be recalculated based on price, tax rate, currency rate</param>
        /// <param name="recalculateBudgetValues">indicates if recalculation of budget invoiced amount should to be done</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        InvoiceLineReturnResult AddLine(InvoiceLineViewModel model, ApplicationUser createdBy, bool recalculateInvoiceValues, bool recalculateBudgetValues);

        /// <summary>
        /// Modify Invoice line based on given InvoiceLineViewModel or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="model">InvoiceLineViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice line</param>
        /// <param name="recalculateInvoiceValues">Indicates if invoice line values should be recalculated based on price, tax rate, currency rate</param>
        /// <param name="recalculateBudgetValues">indicates if recalculation of budget invoiced amount should to be done</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        InvoiceLineReturnResult EditLine(InvoiceLineViewModel model, ApplicationUser modifiedBy, bool recalculateInvoiceValues, bool recalculateBudgetValues);

        /// <summary>
        /// Changes status for given invoice line
        /// </summary>
        /// <param name="lineId">Invoice line id</param>
        /// <param name="invoiceId">invoice id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice line</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        InvoiceLineReturnResult ChangeLineStatus(string lineId, string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        InvoiceLineReturnResult CancelInvoiceLine(string lineId, string invoiceId, ApplicationUser modifiedBy);

        /// <summary>
        /// Gets next invoice line number for given invoice id
        /// </summary>
        /// <param name="invoiceId">invoice id</param>
        /// <returns>next invoice line number for given invoice id</returns>
        int GetNextInvoiceLineNum(string invoiceId);

        /// <summary>
        /// Gets collection of SelectListItem for select field in View with opened customers
        /// </summary>
        /// <param name="selectedCustomer">Customer which will be selected</param>
        /// <returns>collection of SelectListItem for select field in View with opened customers</returns>
        IEnumerable<SelectListItem> GetOpenCustomersItemList(CustomerViewModel selectedCustomer = null);

        /// <summary>
        /// Gets collection of SelectListItem for select field in View with opened budgets
        /// </summary>
        /// <param name="selectedBudget">Budget which will be selected</param>
        /// <returns>collection of SelectListItem for select field in View with opened budgets</returns>
        IEnumerable<SelectListItem> GetOpenBudgetsItemList(BudgetViewModel selectedBudget = null);

        /// <summary>
        /// Gets collection of Invoice models for given customer id
        /// </summary>
        /// <param name="customerId">customer Id for which invoices should be retrieved</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection of invoices for given customer id</returns>
        IEnumerable<Invoice> GetInvoicesForCustomer(string customerId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets collection of InvoiceView models for given customer id
        /// </summary>
        /// <param name="customerId">customer Id for which invoices should be retrieved</param>
        /// <returns>collection of InvoiceView models for given customer id</returns>
        IEnumerable<InvoiceViewModel> GetInvoiceViewModelsForCustomer(string customerId);

        /// <summary>
        /// Gets collection of Invoice line models for given budget id
        /// </summary>
        /// <param name="budgetId">budget Id for which invoice liens should be retrieved</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection of invoices for given customer</returns>
        IEnumerable<InvoiceLine> GetInvoiceLinesForBudget(string budgetId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets collection of InvoiceLineView models for given customer id
        /// </summary>
        /// <param name="budgetId">budget Id for which invoice lines should be retrieved</param>
        /// <returns>collection of InvoiceLineView models for given customer id</returns>
        IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModelsForBudget(string budgetId);
    }
}