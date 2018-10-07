using System;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Helpers
{
    public class MapHelper
    {
        //do zrobienia metoda generyczna

        public BudgetViewModel MapBudgetToViewModel(Budget model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var viewModel = new BudgetViewModel()
            {
                Id = model.Id,
                Status = model.Status,
                BudgetNumber = model.BudgetNumber,
                Description = model.Description,
                CommitedAmount = model.CommitedAmount,
                InvoicedAmount = model.InvoicedAmount,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                Owner = model.Owner
            };

            return viewModel;
        }

        public CustomerViewModel MapCustomerToViewModel(Customer model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var viewModel = new CustomerViewModel()
            {
                Id = model.Id,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
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

            return viewModel;
        }

        public InvoiceViewModel MapInvoiceToViewModel(Invoice model)
        {
            if (model?.Customer == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var viewModel = new InvoiceViewModel()
            {
                Id = model.Id,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                InvoiceNumber = model.InvoiceNumber,
                PaymentMethod = model.PaymentMethod,
                PaymentDueDate = model.PaymentDueDate,
                IssueDate = model.IssueDate,
                ReceiveDate = model.ReceiveDate,
                Customer = MapCustomerToViewModel(model.Customer),
                Currency = model.Currency,
                Budget = model.Budget == null ? null : MapBudgetToViewModel(model.Budget)
            };

            return viewModel;
        }

        public InvoiceLineViewModel MapInvoiceLineToViewModel(InvoiceLine model)
        {
            if (model == null || model.Invoice == null || model.Budget == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var viewModel = new InvoiceLineViewModel()
            {
                Id = model.Id,
                InvoiceId = model.InvoiceId,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                LastModifiedBy = model.LastModifiedBy,
                LastModifiedDate = model.LastModifiedDate,
                Invoice = MapInvoiceToViewModel(model.Invoice),
                LineNumber = model.LineNumber,
                ItemName = model.ItemName,
                Description = model.Description,
                Quantity = model.Quantity,
                Price = model.Price,
                Currency = model.Currency,
                CurrencyRate = model.CurrencyRate,
                TaxRate = model.TaxRate,
                Netto = model.Netto,
                Tax = model.Tax,
                Gross = model.Gross,
                BaseNetto = model.BaseNetto,
                BaseTax = model.BaseTax,
                BaseGross = model.BaseGross,
                Budget = MapBudgetToViewModel(model.Budget)
            };

            return viewModel;
        }
    }
}