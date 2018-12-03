using System.Collections.Generic;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface ICustomerManager
    {
        /// <summary>
        /// Gets list of Customers models
        /// </summary>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with CustomerViewModels</returns>
        IEnumerable<Customer> GetCustomers(IncludeLevel includeLevel);

        /// <summary>
        /// Gets list of CustomerViewModel
        /// </summary>
        /// <returns>collection with CustomerViewModels</returns>
        IEnumerable<CustomerViewModel> GetCustomerViewModels();

        IEnumerable<CustomerViewModel> GetCustomerViewModelsForUser(ApplicationUser user);

        /// <summary>
        /// Gets Customer for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Customer based on customer for given Id</returns>
        Customer GetCustomerById(string customerId, IncludeLevel includeLevel);

        /// <summary>
        /// Gets CustomerViewModel based on budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns>BudgetViewModel based on customer for given Id</returns>
        CustomerViewModel GetCustomerViewModelById(string customerId);

        CustomerViewModel GetCustomerViewModelByIdForUser(string customerId, ApplicationUser user);

        /// <summary>
        /// Add Customer from given CustomerViewModel.
        /// </summary>
        /// <param name="model">CustomerViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        CustomerReturnResult Add(CustomerViewModel model, ApplicationUser createdBy);

        /// <summary>
        /// Modify Customer from given CustomerViewModel or throws exceptions if customer not found.
        /// </summary>
        /// <param name="model">CustomerViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        CustomerReturnResult Edit(CustomerViewModel model, ApplicationUser modifiedBy);

        /// <summary>
        /// Changes status for given customer.
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        CustomerReturnResult ChangeStatus(string customerId, Status newStatus, ApplicationUser modifiedBy);

        CustomerReturnResult Close(string customerId, ApplicationUser modifiedBy);

        CustomerReturnResult Open(string customerId, ApplicationUser modifiedBy);
    }
}