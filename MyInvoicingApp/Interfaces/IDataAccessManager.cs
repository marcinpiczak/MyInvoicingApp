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
    }

    public class DataAccessManager : IDataAccessManager
    {
        //budget default condition
        private Func<BudgetViewModel, ApplicationUser, bool> _budgetDefaultViewCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<BudgetViewModel, ApplicationUser, bool> _budgetDefaultEditCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<BudgetViewModel, ApplicationUser, bool> _budgetDefaultCloseCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<BudgetViewModel, ApplicationUser, bool> _budgetDefaultOpenCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;

        //customer default condition
        private Func<CustomerViewModel, ApplicationUser, bool> _customerDefaultViewCondition = (model, user) => true;
        private Func<CustomerViewModel, ApplicationUser, bool> _customerDefaultEditCondition = (model, user) => true;
        private Func<CustomerViewModel, ApplicationUser, bool> _customerDefaultCloseCondition = (model, user) => true;
        private Func<CustomerViewModel, ApplicationUser, bool> _customerDefaultOpenCondition = (model, user) => true;

        //invoice default condition
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultViewCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultEditCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultCancelCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultApproveCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultRejectCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;
        private Func<InvoiceViewModel, ApplicationUser, bool> _invoiceDefaultSentToApproveCondition = (model, user) => model.Owner.UserName == user.UserName || model.Owner.Manager.UserName == user.UserName;


        //budget
        public bool CanView(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> viewCondition = null)
        {
            if (viewCondition == null)
            {
                return _budgetDefaultViewCondition.Invoke(model, user);
            }

            return viewCondition.Invoke(model, user);
        }

        public bool CanEdit(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> editCondition = null)
        {
            if (editCondition == null)
            {
                return _budgetDefaultEditCondition.Invoke(model, user);
            }

            return editCondition.Invoke(model, user);
        }

        public bool CanClose(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> closeCondition = null)
        {
            if (closeCondition == null)
            {
                return _budgetDefaultCloseCondition.Invoke(model, user);
            }

            return closeCondition.Invoke(model, user);
        }

        public bool CanOpen(BudgetViewModel model, ApplicationUser user, Func<BudgetViewModel, ApplicationUser, bool> openCondition = null)
        {
            if (openCondition == null)
            {
                return _budgetDefaultOpenCondition.Invoke(model, user);
            }

            return openCondition.Invoke(model, user);
        }

        //customer
        public bool CanView(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> viewCondition = null)
        {
            if (viewCondition == null)
            {
                return _customerDefaultViewCondition.Invoke(model, user);
            }

            return viewCondition.Invoke(model, user);
        }

        public bool CanEdit(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> editCondition = null)
        {
            if (editCondition == null)
            {
                return _customerDefaultEditCondition.Invoke(model, user);
            }

            return editCondition.Invoke(model, user);
        }

        public bool CanClose(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> closeCondition = null)
        {
            if (closeCondition == null)
            {
                return _customerDefaultCloseCondition.Invoke(model, user);
            }

            return closeCondition.Invoke(model, user);
        }

        public bool CanOpen(CustomerViewModel model, ApplicationUser user, Func<CustomerViewModel, ApplicationUser, bool> openCondition = null)
        {
            if (openCondition == null)
            {
                return _customerDefaultOpenCondition.Invoke(model, user);
            }

            return openCondition.Invoke(model, user);
        }

        //invoice
        public bool CanView(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null)
        {
            if (viewCondition == null)
            {
                return _invoiceDefaultViewCondition.Invoke(model, user);
            }

            return viewCondition.Invoke(model, user);
        }

        public bool CanEdit(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null)
        {
            if (editCondition == null)
            {
                return _invoiceDefaultEditCondition.Invoke(model, user);
            }

            return editCondition.Invoke(model, user);
        }

        public bool CanCancel(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null)
        {
            if (cancelCondition == null)
            {
                return _invoiceDefaultCancelCondition.Invoke(model, user);
            }

            return cancelCondition.Invoke(model, user);
        }

        public bool CanApprove(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null)
        {
            if (approveCondition == null)
            {
                return _invoiceDefaultApproveCondition.Invoke(model, user);
            }

            return approveCondition.Invoke(model, user);
        }

        public bool CanReject(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null)
        {
            if (rejectCondition == null)
            {
                return _invoiceDefaultRejectCondition.Invoke(model, user);
            }

            return rejectCondition.Invoke(model, user);
        }

        public bool CanSentToApprove(InvoiceViewModel model, ApplicationUser user, Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null)
        {
            if (sentCondition == null)
            {
                return _invoiceDefaultSentToApproveCondition.Invoke(model, user);
            }

            return sentCondition.Invoke(model, user);
        }
    }
}