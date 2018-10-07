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

        IEnumerable<Invoice> GetInvoices();

        InvoiceReturnResult Add(InvoiceViewModel model, ApplicationUser createdBy);

        InvoiceLineReturnResult AddLine(InvoiceLineViewModel model, ApplicationUser createdBy, bool recalculateValues, bool recalculateBudgetValues);

        InvoiceReturnResult Edit(InvoiceViewModel model, ApplicationUser modifiedBy);

        InvoiceLineReturnResult EditLine(InvoiceLineViewModel model, ApplicationUser modifiedBy, bool recalculateValues, bool recalculateBudgetValues);

        InvoiceViewModel GetInvoiceViewModelById(string invoiceId);

        Invoice GetInvoiceById(string invoiceId);

        Invoice GetInvoiceByIdSimple(string invoiceId);

        InvoiceLine GetInvoiceLineById(string lineId, string invoiceId);

        IEnumerable<InvoiceLine> GetInvoiceLines(string invoiceId);

        IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModels(string invoiceId);

        void ChangeStatus(string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        void ChangeLineStatus(string lineId, string invoiceId, Status newStatus, ApplicationUser modifiedBy);

        InvoiceViewModel GetDefaultInvoiceViewModelForAdd(string defaultCurrency = "PLN");

        IEnumerable<SelectListItem> GetOpenCustomersItemList(CustomerViewModel selectedCustomer = null);

        IEnumerable<SelectListItem> GetOpenBudgetsItemList(BudgetViewModel selectedBudget = null);

        int GetNextInvoiceLineNum(string invoiceId);
    }
}