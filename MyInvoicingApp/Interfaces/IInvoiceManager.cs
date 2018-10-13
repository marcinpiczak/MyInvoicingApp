using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IInvoiceManager
    {
        IEnumerable<InvoiceViewModel> GetInvoiceViewModels();

        IEnumerable<Invoice> GetInvoices(IncludeLevel includeLevel);

        InvoiceReturnResult Add(InvoiceViewModel model, ApplicationUser createdBy);

        InvoiceLineReturnResult AddLine(InvoiceLineViewModel model, ApplicationUser createdBy, bool recalculateValues, bool recalculateBudgetValues);

        InvoiceReturnResult Edit(InvoiceViewModel model, ApplicationUser modifiedBy);

        InvoiceLineReturnResult EditLine(InvoiceLineViewModel model, ApplicationUser modifiedBy, bool recalculateValues, bool recalculateBudgetValues);

        InvoiceViewModel GetInvoiceViewModelById(string invoiceId);

        Invoice GetInvoiceById(string invoiceId, IncludeLevel includeLevel);

        //Invoice GetInvoiceByIdSimple(string invoiceId);

        InvoiceLine GetInvoiceLineById(string lineId, string invoiceId, IncludeLevel includeLevel);

        IEnumerable<InvoiceLine> GetInvoiceLines(string invoiceId, IncludeLevel includeLevel);

        IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModels(string invoiceId);

        InvoiceReturnResult ChangeStatus(string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        InvoiceLineReturnResult ChangeLineStatus(string lineId, string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        InvoiceViewModel GetDefaultInvoiceViewModelForAdd(string defaultCurrency = "PLN");

        IEnumerable<SelectListItem> GetOpenCustomersItemList(CustomerViewModel selectedCustomer = null);

        IEnumerable<SelectListItem> GetOpenBudgetsItemList(BudgetViewModel selectedBudget = null);

        int GetNextInvoiceLineNum(string invoiceId);
    }
}