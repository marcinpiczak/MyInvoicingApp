using System.Collections.Generic;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface ICustomerManager
    {
        IEnumerable<CustomerViewModel> GetCustomerViewModels();

        IEnumerable<Customer> GetCustomers();

        void Add(CustomerViewModel model, ApplicationUser createdBy);

        void Edit(CustomerViewModel model, ApplicationUser modifiedBy);

        CustomerViewModel GetCustomerViewModelById(string id);

        Customer GetCustomerById(string id);

        IEnumerable<Invoice> GetInvoices(string id);

        IEnumerable<InvoiceViewModel> GetInvoiceViewModels(string id);

        void ChangeStatus(string id, Status newStatus, ApplicationUser modifiedBy);
    }
}