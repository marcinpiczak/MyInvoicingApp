using System;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
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

        public BudgetViewModel GetBudgetViewModelAccess(BudgetViewModel model, ApplicationUser user, 
            Func<BudgetViewModel, ApplicationUser, bool> viewCondition = null, 
            Func<BudgetViewModel, ApplicationUser, bool> editCondition = null, 
            Func<BudgetViewModel, ApplicationUser, bool> closeCondition = null, 
            Func<BudgetViewModel, ApplicationUser, bool> openCondition = null)
        {
            model.Accesses = new AccessViewModel()
            {
                CanView = CanView(model, user, viewCondition),
                CanEdit = CanEdit(model, user, editCondition),
                CanClose = CanClose(model, user, closeCondition),
                CanOpen = CanOpen(model, user, openCondition)
            };

            return model;
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

        public CustomerViewModel GetCustomerViewModelAccess(CustomerViewModel model, ApplicationUser user,
            Func<CustomerViewModel, ApplicationUser, bool> viewCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> editCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> closeCondition = null,
            Func<CustomerViewModel, ApplicationUser, bool> openCondition = null)
        {
            model.Accesses = new AccessViewModel()
            {
                CanView = CanView(model, user, viewCondition),
                CanEdit = CanEdit(model, user, editCondition),
                CanClose = CanClose(model, user, closeCondition),
                CanOpen = CanOpen(model, user, openCondition)
            };

            return model;
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

        public InvoiceViewModel GetInvoiceViewModelAccess(InvoiceViewModel model, ApplicationUser user,
            Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null)
        {
            model.Accesses = new AccessViewModel()
            {
                CanView = CanView(model, user, viewCondition),
                CanEdit = CanEdit(model, user, editCondition),
                CanCancel = CanCancel(model, user, cancelCondition),
                CanApprove = CanApprove(model, user, approveCondition),
                CanReject = CanReject(model, user, rejectCondition),
                CanSentToApprove = CanSentToApprove(model, user, sentCondition)
            };

            return model;
        }

        public InvoiceLineViewModel GetInvoiceLineViewModelAccess(InvoiceLineViewModel model, ApplicationUser user,
            Func<InvoiceViewModel, ApplicationUser, bool> viewCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> editCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> cancelCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> approveCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> rejectCondition = null,
            Func<InvoiceViewModel, ApplicationUser, bool> sentCondition = null)
        {
            model.Invoice.Accesses = new AccessViewModel()
            {
                CanView = CanView(model.Invoice, user, viewCondition),
                CanEdit = CanEdit(model.Invoice, user, editCondition),
                CanCancel = CanCancel(model.Invoice, user, cancelCondition),
                CanApprove = CanApprove(model.Invoice, user, approveCondition),
                CanReject = CanReject(model.Invoice, user, rejectCondition),
                CanSentToApprove = CanSentToApprove(model.Invoice, user, sentCondition)
            };

            return model;
        }
    }
}