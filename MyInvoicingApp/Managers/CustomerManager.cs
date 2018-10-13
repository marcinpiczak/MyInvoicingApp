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
    public class CustomerManager :IManager, ICustomerManager
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

        public IEnumerable<CustomerViewModel> GetCustomerViewModels()
        {
            var models = Context.Customers
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .Select(x => new CustomerViewModel(x));

            return models;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var customers = Context.Customers
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy);

            return customers;
        }

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

        public CustomerReturnResult Edit(CustomerViewModel model, ApplicationUser modifiedBy)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException("model", "Nieprawidłowe parametry");
            }

            var customer = GetCustomerById(model.Id);

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

        public CustomerViewModel GetCustomerViewModelById(string id)
        {
            var customer = GetCustomerById(id);

            if (customer == null)
            {
                throw new ArgumentException("Bark klienta o podanym Id");
            }

            var model = new CustomerViewModel(customer);

            return model;
        }

        public Customer GetCustomerById(string id)
        {
            var customer = Context.Customers
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .FirstOrDefault(x => x.Id == id);

            if (customer == null)
            {
                throw new ArgumentException("Bark klienta o podanym Id");
            }

            //Context.Entry(customer).State = EntityState.Detached;

            return customer;
        }

        public IEnumerable<Invoice> GetInvoices(string id)
        {
            var invoicesList = Context.Invoices
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                .Where(x => x.CustomerId == id);

            return invoicesList;
        }

        public IEnumerable<InvoiceViewModel> GetInvoiceViewModels(string id)
        {
            var models = GetInvoices(id)
                .Select(x => new InvoiceViewModel(x));

            return models;
        }

        public CustomerReturnResult ChangeStatus(string id, Status newStatus, ApplicationUser modifiedBy)
        {
            if (modifiedBy == null)
            {
                throw new ArgumentNullException("modifiedBy", "Nieprawidłowe parametry");
            }

            var customer = GetCustomerById(id);
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