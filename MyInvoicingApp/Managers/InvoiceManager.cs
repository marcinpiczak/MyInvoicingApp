using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ReturnResults;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class InvoiceManager : IManager, IInvoiceManager
    {
        protected EFCDbContext Context { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected DateHelper DateHelper { get; set; }
        protected IDocumentNumberingManager DocumentNumberingManager { get; set; }
        protected ICustomerManager CustomerManager { get; set; }
        protected IBudgetManager BudgetManager { get; set; }

        public InvoiceManager(EFCDbContext context, UserManager<ApplicationUser> userManager, DateHelper dateHelper, ICustomerManager customerManager, IBudgetManager budgetManager, IDocumentNumberingManager documentNumberingManager)
        {
            Context = context;
            UserManager = userManager;
            DateHelper = dateHelper;
            DocumentNumberingManager = documentNumberingManager;
            CustomerManager = customerManager;
            BudgetManager = budgetManager;
        }

        public IEnumerable<InvoiceViewModel> GetInvoiceViewModels()
        {
            var models = GetInvoices()
                //.Select(x => new InvoiceViewModel()
                //{
                //    Id = x.Id,
                //    Status = x.Status,
                //    CreatedBy = x.CreatedBy,
                //    CreatedDate = x.CreatedDate,
                //    LastModifiedBy = x.LastModifiedBy,
                //    LastModifiedDate = x.LastModifiedDate,
                //    InvoiceNumber = x.InvoiceNumber,
                //    PaymentMethod = x.PaymentMethod,
                //    PaymentDueDate = x.PaymentDueDate,
                //    IssueDate = x.IssueDate,
                //    ReceiveDate = x.ReceiveDate,
                //    Customer = x.Customer,
                //    Currency = x.Currency,
                //    Budget = x.Budget
                //})
                .Select(x => new InvoiceViewModel(x))
                .OrderByDescending(x => x.CreatedDate);

            return models;
        }

        public IEnumerable<Invoice> GetInvoices()
        {
            var invoices = Context.Invoices
                .Include(x => x.Customer).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Customer).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.Owner)
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy);

            return invoices;
        }

        public InvoiceReturnResult Add(InvoiceViewModel model, ApplicationUser createdBy)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var invoice = Context.Invoices.SingleOrDefault(x => x.Id == model.Id);

            if (invoice != null)
            {
                throw new ArgumentException($"Faktura o podanym ID już istnieje. Nie można utworzyć faktury z takim samym ID", nameof(invoice));
            }

            var now = DateHelper.GetCurrentDatetime();

            var documentNumber = DocumentNumberingManager.GetNextDocumentNumber(DocumentType.Invoice, now, createdBy);

            invoice = Context.Invoices.SingleOrDefault(x => x.InvoiceNumber == documentNumber);

            if (invoice != null)
            {
                throw new ArgumentException($"Faktura o podanym numerze {documentNumber} już istnieje. Spróbuj ponownie a jeżeli problem będzie się powtarzał przestaw ręcznie numerację", nameof(invoice));
            }

            var newInvoice = new Invoice()
            {
                Status = Status.Opened,
                CreatedById = createdBy.Id,
                CreatedDate = now,
                InvoiceNumber = documentNumber,
                PaymentMethod = model.PaymentMethod,
                PaymentDueDate = model.PaymentDueDate,
                IssueDate = model.IssueDate,
                ReceiveDate = model.ReceiveDate,
                CustomerId = model.CustomerId,
                Currency = model.Currency,
                BudgetId = model.BudgetId
            };

            Context.Invoices.Add(newInvoice);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            //return new []{ newInvoice.Id , newInvoice.InvoiceNumber, newInvoice.Status.ToString()};
            return new InvoiceReturnResult()
            {
                Id = newInvoice.Id,
                InvoiceNumber = newInvoice.InvoiceNumber,
                Status = newInvoice.Status.ToString()
            };
        }

        public InvoiceLineReturnResult AddLine(InvoiceLineViewModel model, ApplicationUser createdBy, bool recalculateInvoiceValues, bool recalculateBudgetValues)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            decimal netto = model.Netto;
            decimal tax = model.Tax;
            decimal gross = model.Gross;
            decimal baseNetto = model.BaseNetto;
            decimal baseTax = model.BaseTax;
            decimal baseGross = model.BaseGross;

            if (recalculateInvoiceValues)
            {
                netto = model.Quantity * model.Price;
                tax = model.Netto * (model.TaxRate / 100);
                gross = netto + tax;

                if (model.Currency.ToUpper() == "PLN")
                {
                    baseNetto = netto;
                    baseTax = tax;
                    baseGross = gross;
                }
                else
                {
                    baseNetto = model.CurrencyRate == 0 ? 0 : netto * model.CurrencyRate;
                    baseTax = baseNetto * (model.TaxRate / 100);
                    baseGross = baseNetto + baseTax;
                }
            }

            var now = DateHelper.GetCurrentDatetime();
            var invoiceLineNumber = GetNextInvoiceLineNum(model.InvoiceId);

            //var budget = BudgetManager.GetBudgetById(model.BudgetId);

            BudgetManager.UpdateBudgetInvoicedAmount(model.BudgetId, baseNetto, recalculateBudgetValues);

            var newInvoiceLine = new InvoiceLine()
            {
                Status = Status.Opened,
                CreatedById = createdBy.Id,
                CreatedDate = now,
                InvoiceId = model.InvoiceId,
                LineNumber = invoiceLineNumber,
                ItemName = model.ItemName,
                Description = model.Description,
                Quantity = model.Quantity,
                Price = model.Price,
                Currency = model.Currency,
                CurrencyRate = model.CurrencyRate,
                TaxRate = model.TaxRate,
                Netto = netto,
                Tax = tax,
                Gross = gross,
                BaseNetto = baseNetto,
                BaseTax = baseTax,
                BaseGross = baseGross,
                BudgetId = model.BudgetId
            };

            Context.InvoiceLines.Add(newInvoiceLine);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            //return new []{ newInvoiceLine.Id, newInvoiceLine.InvoiceId, newInvoiceLine.LineNumber.ToString(), newInvoiceLine.Status.ToString() };
            return new InvoiceLineReturnResult()
            {
                Id = newInvoiceLine.Id,
                InvoiceId = newInvoiceLine.InvoiceId,
                LineNumber = newInvoiceLine.LineNumber,
                Status = newInvoiceLine.Status.ToString()
            };
        }

        public InvoiceReturnResult Edit(InvoiceViewModel model, ApplicationUser modifiedBy)
        {
            var invoice = Context.Invoices.SingleOrDefault(x => x.Id == model.Id);

            if (invoice == null)
            {
                throw new ArgumentException($"Faktura o podanym ID nie istnieje.", nameof(invoice));
            }

            if (model.BudgetId != "")
            {
                var budget = Context.Budgets.SingleOrDefault(x => x.Id == model.BudgetId);

                if (budget == null)
                {
                    throw new ArgumentException($"Budżet o podanym ID nie istnieje.", nameof(budget));
                }
            }

            var customer = Context.Customers.SingleOrDefault(x => x.Id == model.CustomerId);

            if (customer == null)
            {
                throw new ArgumentException($"Klient o podanym ID nie istnieje.", nameof(customer));
            }

            invoice.LastModifiedById = modifiedBy.Id;
            invoice.ReferenceNumber = model.ReferenceNumber;
            invoice.PaymentMethod = model.PaymentMethod;
            invoice.PaymentDueDate = model.PaymentDueDate;
            invoice.IssueDate = model.IssueDate;
            invoice.ReceiveDate = model.ReceiveDate;
            invoice.CustomerId = model.CustomerId;
            invoice.Currency = model.Currency;
            invoice.BudgetId = model.BudgetId;
            
            Context.Invoices.Update(invoice);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            return new InvoiceReturnResult()
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Status = invoice.Status.ToString()
            };
        }

        public InvoiceLineReturnResult EditLine(InvoiceLineViewModel model, ApplicationUser modifiedBy, bool recalculateInvoiceValues, bool recalculateBudgetValues)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var invoiceLine = GetInvoiceLineById(model.Id, model.InvoiceId);

            decimal netto = model.Netto;
            decimal tax = model.Tax;
            decimal gross = model.Gross;
            decimal baseNetto = model.BaseNetto;
            decimal baseTax = model.BaseTax;
            decimal baseGross = model.BaseGross;

            if (recalculateInvoiceValues)
            {
                netto = model.Quantity * model.Price;
                tax = model.Netto * (model.TaxRate / 100);
                gross = netto + tax;

                if (model.Currency.ToUpper() == "PLN")
                {
                    baseNetto = netto;
                    baseTax = tax;
                    baseGross = gross;
                }
                else
                {
                    baseNetto = model.CurrencyRate == 0 ? 0 : netto * model.CurrencyRate;
                    baseTax = baseNetto * (model.TaxRate / 100);
                    baseGross = baseNetto + baseTax;
                }
            }

            var now = DateHelper.GetCurrentDatetime();


            if (invoiceLine.BudgetId != model.BudgetId)
            {
                BudgetManager.UpdateBudgetInvoicedAmount(invoiceLine.BudgetId, -baseNetto, recalculateBudgetValues);
                BudgetManager.UpdateBudgetInvoicedAmount(model.BudgetId, baseNetto, recalculateBudgetValues);
            }

            invoiceLine.LastModifiedById = modifiedBy.Id;
            invoiceLine.LastModifiedDate = now;
            invoiceLine.ItemName = model.ItemName;
            invoiceLine.Description = model.Description;
            invoiceLine.Quantity = model.Quantity;
            invoiceLine.Price = model.Price;
            invoiceLine.Currency = model.Currency;
            invoiceLine.CurrencyRate = model.CurrencyRate;
            invoiceLine.TaxRate = model.TaxRate;
            invoiceLine.Netto = netto;
            invoiceLine.Tax = tax;
            invoiceLine.Gross = gross;
            invoiceLine.BaseNetto = baseNetto;
            invoiceLine.BaseTax = baseTax;
            invoiceLine.BaseGross = baseGross;
            invoiceLine.BudgetId = model.BudgetId;

            Context.InvoiceLines.Update(invoiceLine);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            //return new []{ newInvoiceLine.Id, newInvoiceLine.InvoiceId, newInvoiceLine.LineNumber.ToString(), newInvoiceLine.Status.ToString() };
            return new InvoiceLineReturnResult()
            {
                Id = invoiceLine.Id,
                InvoiceId = invoiceLine.InvoiceId,
                LineNumber = invoiceLine.LineNumber,
                Status = invoiceLine.Status.ToString()
            };
        }

        public InvoiceViewModel GetInvoiceViewModelById(string invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);

            var customerItemList = GetOpenCustomersItemList(invoice.Customer == null ? null : new CustomerViewModel(invoice.Customer));
            var budgetItemList = GetOpenBudgetsItemList(invoice.Budget == null ? null : new BudgetViewModel(invoice.Budget));

            //var model = new InvoiceViewModel()
            //{
            //    Id = invoice.Id,
            //    Status = invoice.Status,
            //    CreatedBy = invoice.CreatedBy,
            //    CreatedDate = invoice.CreatedDate,
            //    LastModifiedBy = invoice.LastModifiedBy,
            //    LastModifiedDate = invoice.LastModifiedDate,
            //    InvoiceNumber = invoice.InvoiceNumber,
            //    PaymentMethod = invoice.PaymentMethod,
            //    PaymentDueDate = invoice.PaymentDueDate,
            //    IssueDate = invoice.IssueDate,
            //    ReceiveDate = invoice.ReceiveDate,
            //    Customer = invoice.Customer,
            //    Currency = invoice.Currency,
            //    Budget = invoice.Budget,
            //    BudgetItemList = budgetItemList,
            //    CustomerItemList = customerItemList,
            //    InvoiceLines = GetInvoiceLineViewModels(invoiceId)
            //};

            var model = new InvoiceViewModel(invoice)
            {
                BudgetItemList = budgetItemList,
                CustomerItemList = customerItemList,
                InvoiceLines = GetInvoiceLineViewModels(invoiceId)
            };

            return model;
        }

        public Invoice GetInvoiceById(string invoiceId)
        {
            var invoice = Context.Invoices
                .Include(x => x.Customer).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Customer).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .SingleOrDefault(x => x.Id == invoiceId);

            if (invoice == null)
            {
                throw new ArgumentException("Bark faktury o podanym Id", nameof(invoice));
            }

            return invoice;
        }

        public Invoice GetInvoiceByIdSimple(string invoiceId)
        {
            var invoice = Context.Invoices
                .SingleOrDefault(x => x.Id == invoiceId);

            if (invoice == null)
            {
                throw new ArgumentException("Bark faktury o podanym Id", nameof(invoice));
            }

            return invoice;
        }

        public InvoiceLine GetInvoiceLineById(string lineId, string invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);

            var invoiceLine = Context.InvoiceLines
                .Include(x => x.Invoice)
                .Include(x => x.Budget)
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .SingleOrDefault(x => x.Id == lineId && x.InvoiceId == invoice.Id);

            if (invoiceLine == null)
            {
                throw new ArgumentException("Linia o podanym Id nie istnieje dla wskazanej faktury", nameof(invoice));
            }

            return invoiceLine;
        }

        public IEnumerable<InvoiceLine> GetInvoiceLines(string invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);

            var invoiceLines = Context.InvoiceLines
                .Include(x => x.Invoice).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Invoice).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                .Include(x => x.Budget).ThenInclude(x => x.Owner)
                .Include(x => x.CreatedBy)
                .Include(x => x.LastModifiedBy)
                .Where(x => x.InvoiceId == invoice.Id);

            return invoiceLines;
        }

        public IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModels(string invoiceId)
        {
            var invoiceLines = GetInvoiceLines(invoiceId);

            var models = invoiceLines
                //.Select(x => new InvoiceLineViewModel()
                //{
                //    Id = x.Id,
                //    InvoiceId = x.InvoiceId,
                //    Status = x.Status,
                //    CreatedBy = x.CreatedBy,
                //    CreatedDate = x.CreatedDate,
                //    LastModifiedBy = x.LastModifiedBy,
                //    LastModifiedDate = x.LastModifiedDate,
                //    Invoice = x.Invoice,
                //    LineNumber = x.LineNumber,
                //    ItemName = x.ItemName,
                //    Description = x.Description,
                //    Quantity = x.Quantity,
                //    Price = x.Price,
                //    Currency = x.Currency,
                //    CurrencyRate = x.CurrencyRate,
                //    TaxRate = x.TaxRate,
                //    Netto = x.Netto,
                //    Tax = x.Tax,
                //    Gross = x.Gross,
                //    BaseNetto = x.BaseNetto,
                //    BaseTax = x.BaseTax,
                //    BaseGross = x.BaseGross,
                //    Budget = x.Budget
                //});
                .Select(x => new InvoiceLineViewModel(x));

            return models;
        }

        public void ChangeStatus(string invoiceId, Status newStatus, ApplicationUser modifiedBy)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeLineStatus(string lineId, string invoiceId, Status newStatus, ApplicationUser modifiedBy)
        {
            var invoiceLine = GetInvoiceLineById(lineId, invoiceId);

            if (invoiceLine.Status == Status.Cancelled && newStatus == Status.Cancelled)
            {
                throw new Exception("Linia została już anulowana");
            }

            var now = DateHelper.GetCurrentDatetime();

            invoiceLine.Status = newStatus;
            invoiceLine.LastModifiedById = modifiedBy.Id;
            invoiceLine.LastModifiedDate = now;

            Context.InvoiceLines.Update(invoiceLine);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

        }

        public InvoiceViewModel GetDefaultInvoiceViewModelForAdd(string defaultCurrency = "PLN")
        {
            var customerItemList = GetOpenCustomersItemList();
            var budgetItemList = GetOpenBudgetsItemList();

            var now = DateHelper.GetCurrentDatetime();

            var model = new InvoiceViewModel()
            {
                CustomerItemList = customerItemList,
                BudgetItemList = budgetItemList,
                PaymentDueDate = now,
                IssueDate = now,
                ReceiveDate = now,
                Currency = defaultCurrency,
                //
                InvoiceLine = new InvoiceLineViewModel()
            };

            return model;
        }

        public IEnumerable<SelectListItem> GetOpenCustomersItemList(CustomerViewModel selectedCustomer = null)
        {
            var customersItemList = CustomerManager.GetCustomers()
                .Where(x => x.Status == Status.Opened)
                .Select(x => new SelectListItem(x.Name, x.Id));

            if (selectedCustomer != null)
            {
                customersItemList = customersItemList
                    .Where(x => x.Value != selectedCustomer.Id)
                    .Union(new List<SelectListItem>() { new SelectListItem(selectedCustomer.Name, selectedCustomer.Id, true) });
            }
            else
            {
                customersItemList = customersItemList.Union(new List<SelectListItem>() { new SelectListItem(null, null, true) });
            }

            return customersItemList.OrderByDescending(x => x.Value);
        }

        public IEnumerable<SelectListItem> GetOpenBudgetsItemList(BudgetViewModel selectedBudget = null)
        {
            var budgetsItemList = BudgetManager.GetBudgets()
                .Where(x => x.Status == Status.Opened)
                .Select(x => new SelectListItem(x.BudgetNumber, x.Id));

            if (selectedBudget != null)
            {
                budgetsItemList = budgetsItemList
                    .Where(x => x.Value != selectedBudget.Id)
                    .Union(new List<SelectListItem>() { new SelectListItem(selectedBudget.BudgetNumber, selectedBudget.Id, true) });
            }
            else
            {
                budgetsItemList = budgetsItemList.Union(new List<SelectListItem>() { new SelectListItem(null, null, true) });
            }

            return budgetsItemList.OrderByDescending(x => x.Value);
        }

        public int GetNextInvoiceLineNum(string invoiceId)
        {
            var invoiceLines = Context.InvoiceLines.Where(x => x.InvoiceId == invoiceId);

            var lineNum = !invoiceLines.Any() ? 0 : invoiceLines.Max(x => x.LineNumber);

            return lineNum + 1;
        }
    }
}