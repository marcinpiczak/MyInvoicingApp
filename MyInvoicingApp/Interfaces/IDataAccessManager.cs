using System;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IDataAccessManager
    {
        bool CanView(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> viewCondition = null);

        bool CanView(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null);

        //bool CanView(InvoiceLine invoiceLine, ApplicationUser user);

        bool CanView(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> viewCondition = null);

        bool CanEdit(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> editCondition = null);

        bool CanEdit(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null);

        //bool CanEdit(InvoiceLine invoiceLine, ApplicationUser user, Func<InvoiceLine, bool> editCondition);

        bool CanEdit(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> editCondition = null);

        bool CanClose(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> closeCondition = null);

        bool CanClose(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> closeCondition = null);

        bool CanOpen(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> openCondition = null);

        bool CanOpen(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> openCondition = null);

        bool CanCancel(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null);

        //bool CanCancel(InvoiceLine invoiceLine, ApplicationUser user, Func<InvoiceLine, bool> cancelCondition);

        bool CanApprove(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null);

        bool CanReject(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null);

        bool CanSentToApprove(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null);

        BudgetViewModel GetBudgetViewModelAccess(BudgetViewModel model, ApplicationUser user,
            Func<BudgetViewModel, ApplicationUser, bool> viewCondition = null,
            Func<BudgetViewModel, ApplicationUser, bool> editCondition = null,
            Func<BudgetViewModel, ApplicationUser, bool> closeCondition = null,
            Func<BudgetViewModel, ApplicationUser, bool> openCondition = null);

        CustomerViewModel GetCustomerViewModelAccess(CustomerViewModel model, ApplicationUser user,
            Func<CustomerViewModel, ApplicationUser, bool> viewCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> editCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> closeCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> openCondition = null);

        InvoiceViewModel GetInvoiceViewModelAccess(InvoiceViewModel model, ApplicationUser user,
            Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null);

        InvoiceLineViewModel GetInvoiceLineViewModelAccess(InvoiceLineViewModel model, ApplicationUser user,
            Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null);
    }
}