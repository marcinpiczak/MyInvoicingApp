using System;
using Microsoft.AspNetCore.Identity;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class CustomerManager : IManager, ICustomerManager
    {
        protected EFCDbContext Context { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected DateHelper DateHelper { get; set; }

        public CustomerManager(EFCDbContext context, UserManager<ApplicationUser> userManager, DateHelper dateHelper)
        {
            Context = context;
            UserManager = userManager;
            DateHelper = dateHelper;
        }

        /// <summary>
        /// Gets list of Customers models
        /// </summary>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with CustomerViewModels</returns>
        public IEnumerable<Customer> GetCustomers(IncludeLevel includeLevel)
        {
            IEnumerable<Customer> customers;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    customers = Context.Customers;
                    break;

                case IncludeLevel.Level1:
                    customers = Context.Customers
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy);
                    break;

                default:
                    customers = Context.Customers
                        .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                        .Include(x => x.LastModifiedBy);
                    break;
            }

            return customers;
        }

        /// <summary>
        /// Gets list of CustomerViewModel
        /// </summary>
        /// <returns>collection with CustomerViewModels</returns>
        public IEnumerable<CustomerViewModel> GetCustomerViewModels()
        {
            var customers = GetCustomers(IncludeLevel.Level1)
                .Select(x => new CustomerViewModel(x));

            return customers;
        }

        /// <summary>
        /// Gets Customer for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Customer based on customer for given Id</returns>
        public Customer GetCustomerById(string id, IncludeLevel includeLevel)
        {
            Customer customer;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    customer = Context.Customers
                        .FirstOrDefault(x => x.Id == id);
                    break;

                case IncludeLevel.Level1:
                    customer = Context.Customers
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .FirstOrDefault(x => x.Id == id);
                    break;

                default:
                    customer = Context.Customers
                        .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                        .Include(x => x.LastModifiedBy)
                        .FirstOrDefault(x => x.Id == id);
                    break;
            }

            if (customer == null)
            {
                throw new ArgumentException("Bark klienta o podanym Id");
            }

            return customer;
        }

        /// <summary>
        /// Gets CustomerViewModel based on budget for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>BudgetViewModel based on customer for given Id</returns>
        public CustomerViewModel GetCustomerViewModelById(string id)
        {
            var customer = GetCustomerById(id, IncludeLevel.Level1);

            if (customer == null)
            {
                throw new ArgumentException("Bark klienta o podanym Id");
            }

            var model = new CustomerViewModel(customer);

            return model;
        }

        /// <summary>
        /// Add Customer from given CustomerViewModel.
        /// </summary>
        /// <param name="model">CustomerViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        public CustomerReturnResult Add(CustomerViewModel model, ApplicationUser createdBy)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException("model", "Nieprawidłowe parametry");
            }

            var newCustomer = new Customer()
            {
                Status = Status.Opened,
                CreatedById = createdBy.Id,
                CreatedDate = DateHelper.GetCurrentDatetime(),
                Name = model.Name,
                Description = model.Description,
                City = model.City,
                PostalCode = model.PostalCode,
                Street = model.Street,
                BuildingNumber = model.BuildingNumber,
                Notes = model.Notes,
                PhoneNumber = model.PhoneNumber,
                DefaultPaymentMethod = model.DefaultPaymentMethod
            };

            Context.Customers.Add(newCustomer);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            return new CustomerReturnResult()
            {
                Id = newCustomer.Id,
                Name = newCustomer.Name,
                Status = newCustomer.Status.ToString()
            };
        }

        /// <summary>
        /// Modify Customer from given CustomerViewModel or throws exceptions if customer not found.
        /// </summary>
        /// <param name="model">CustomerViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        public CustomerReturnResult Edit(CustomerViewModel model, ApplicationUser modifiedBy)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException("model", "Nieprawidłowe parametry");
            }

            var customer = GetCustomerById(model.Id, IncludeLevel.None);

            customer.Name = model.Name;
            customer.Description = model.Description;
            customer.City = model.City;
            customer.PostalCode = model.PostalCode;
            customer.Street = model.Street;
            customer.BuildingNumber = model.BuildingNumber;
            customer.Notes = model.Notes;
            customer.PhoneNumber = model.PhoneNumber;
            customer.DefaultPaymentMethod = model.DefaultPaymentMethod;
            customer.LastModifiedById = modifiedBy.Id;
            customer.LastModifiedDate = DateHelper.GetCurrentDatetime();

            Context.Customers.Update(customer);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            return new CustomerReturnResult()
            {
                Id = customer.Id,
                Name = customer.Name,
                Status = customer.Status.ToString()
            };
        }

        /// <summary>
        /// Changes status for given customer.
        /// </summary>
        /// <param name="id">customer id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying customer</param>
        /// <returns>CustomerReturnResult with id, name and status</returns>
        public CustomerReturnResult ChangeStatus(string id, Status newStatus, ApplicationUser modifiedBy)
        {
            if (modifiedBy == null)
            {
                throw new ArgumentNullException("modifiedBy", "Nieprawidłowe parametry");
            }

            var customer = GetCustomerById(id, IncludeLevel.None);
            customer.Status = newStatus;
            customer.LastModifiedById = modifiedBy.Id;
            customer.LastModifiedDate = DateHelper.GetCurrentDatetime();

            Context.Customers.Update(customer);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            return new CustomerReturnResult()
            {
                Id = customer.Id,
                Name = customer.Name,
                Status = customer.Status.ToString()
            };
        }
    }
}