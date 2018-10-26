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
        protected IDateHelper DateHelper { get; set; }
        protected IDocumentNumberingManager DocumentNumberingManager { get; set; }
        protected ICustomerManager CustomerManager { get; set; }
        protected IBudgetManager BudgetManager { get; set; }

        public InvoiceManager(EFCDbContext context, UserManager<ApplicationUser> userManager, IDateHelper dateHelper, ICustomerManager customerManager, IBudgetManager budgetManager, IDocumentNumberingManager documentNumberingManager)
        {
            Context = context;
            UserManager = userManager;
            DateHelper = dateHelper;
            DocumentNumberingManager = documentNumberingManager;
            CustomerManager = customerManager;
            BudgetManager = budgetManager;
        }

        /// <summary>
        /// Gets collection of Invoice models
        /// </summary>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with Invoice models</returns>
        public IEnumerable<Invoice> GetInvoices(IncludeLevel includeLevel)
        {
            IEnumerable<Invoice> invoices;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoices = Context.Invoices;
                    break;

                case IncludeLevel.Level1:
                    invoices = Context.Invoices
                        .Include(x => x.Customer)
                        .Include(x => x.Customer)
                        .Include(x => x.Budget)
                        .Include(x => x.Budget)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy);
                    break;

                default:
                    invoices = Context.Invoices
                        .Include(x => x.Customer).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Customer).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy);
                    break;
            }

            return invoices;
        }

        /// <summary>
        /// Gets collection of InvoiceViewModels
        /// </summary>
        /// <returns>collection with InvoiceViewModels</returns>
        public IEnumerable<InvoiceViewModel> GetInvoiceViewModels()
        {
            var models = GetInvoices(IncludeLevel.Level2)
                .Select(x => new InvoiceViewModel(x));

            return models;
        }

        /// <summary>
        /// Gets Invoice for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        public Invoice GetInvoiceById(string invoiceId, IncludeLevel includeLevel)
        {
            Invoice invoice;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoice = Context.Invoices
                        .SingleOrDefault(x => x.Id == invoiceId);
                    break;

                case IncludeLevel.Level1:
                    invoice = Context.Invoices
                        .Include(x => x.Customer)
                        .Include(x => x.Budget)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .SingleOrDefault(x => x.Id == invoiceId);
                    break;

                default:
                    invoice = Context.Invoices
                        .Include(x => x.Customer).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Customer).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .SingleOrDefault(x => x.Id == invoiceId);
                    break;
            }

            if (invoice == null)
            {
                throw new ArgumentException("Brak faktury o podanym Id", nameof(invoice));
            }

            return invoice;
        }

        /// <summary>
        /// Gets InvoiceViewModel based on invoice for given Id or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <returns>InvoiceViewModel based on invoice for given Id</returns>
        public InvoiceViewModel GetInvoiceViewModelById(string invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId, IncludeLevel.Level2);

            var customerItemList = GetOpenCustomersItemList(invoice.Customer == null ? null : new CustomerViewModel(invoice.Customer));
            var budgetItemList = GetOpenBudgetsItemList(invoice.Budget == null ? null : new BudgetViewModel(invoice.Budget));

            var model = new InvoiceViewModel(invoice)
            {
                BudgetItemList = budgetItemList,
                CustomerItemList = customerItemList,
                InvoiceLines = GetInvoiceLineViewModels(invoiceId)
            };

            return model;
        }

        /// <summary>
        /// Add Invoice based on given InvoiceViewModel.
        /// </summary>
        /// <param name="model">InvoiceViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates Invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        public InvoiceReturnResult Add(InvoiceViewModel model, ApplicationUser createdBy)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var now = DateHelper.GetCurrentDatetime();

            var documentNumber = DocumentNumberingManager.GetNextDocumentNumber(DocumentType.Invoice, now, createdBy);

            var invoice = Context.Invoices.SingleOrDefault(x => x.InvoiceNumber == documentNumber);

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

            return new InvoiceReturnResult()
            {
                Id = newInvoice.Id,
                InvoiceNumber = newInvoice.InvoiceNumber,
                Status = newInvoice.Status.ToString()
            };
        }

        /// <summary>
        /// Modify Invoice based on given InvoiceViewModel or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="model">InvoiceViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        public InvoiceReturnResult Edit(InvoiceViewModel model, ApplicationUser modifiedBy)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var invoice = GetInvoiceById(model.Id, IncludeLevel.None);

            if (invoice.Status == Status.Cancelled)
            {
                throw new InvalidOperationException("Fatura jest anulowana. Nie można edytować anulowanej faktury");
            }

            if (!string.IsNullOrWhiteSpace(model.BudgetId))
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

            var now = DateHelper.GetCurrentDatetime();

            invoice.LastModifiedById = modifiedBy.Id;
            invoice.LastModifiedDate = now;
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

        /// <summary>
        /// Changes status for given invoice
        /// </summary>
        /// <param name="invoiceId">invoice id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice</param>
        /// <returns>InvoiceReturnResult with id, invoice number and status</returns>
        public InvoiceReturnResult ChangeStatus(string invoiceId, Status newStatus, ApplicationUser modifiedBy)
        {
            var invoice = GetInvoiceById(invoiceId, IncludeLevel.None);
            var invoiceLines = GetInvoiceLines(invoiceId, IncludeLevel.None).ToList();

            var now = DateHelper.GetCurrentDatetime();

            invoice.Status = newStatus;
            invoice.LastModifiedById = modifiedBy.Id;
            invoice.LastModifiedDate = now;

            invoiceLines.ForEach(x =>
            {
                x.Status = newStatus;
                x.LastModifiedById = modifiedBy.Id;
                x.LastModifiedDate = now;
            });

            Context.Invoices.Update(invoice);
            Context.InvoiceLines.UpdateRange(invoiceLines);
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

        /// <summary>
        /// Gets InvoiceViewModel with default values set for Add method
        /// </summary>
        /// <param name="defaultCurrency">Default currency</param>
        /// <returns>InvoiceViewModel with default values set</returns>
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

        /// <summary>
        /// Gets collection of Invoice line models for given invoice Id
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection with Invoice models</returns>
        public IEnumerable<InvoiceLine> GetInvoiceLines(string invoiceId, IncludeLevel includeLevel)
        {
            var invoice = GetInvoiceById(invoiceId, IncludeLevel.None);

            IEnumerable<InvoiceLine> invoiceLines;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoiceLines = Context.InvoiceLines
                        .Where(x => x.InvoiceId == invoice.Id);
                    break;

                case IncludeLevel.Level1:
                    invoiceLines = Context.InvoiceLines
                        .Include(x => x.Invoice)
                        .Include(x => x.Budget)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .Where(x => x.InvoiceId == invoice.Id);
                    break;

                default:
                    invoiceLines = Context.InvoiceLines
                        .Include(x => x.Invoice).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Invoice).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.Owner)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .Where(x => x.InvoiceId == invoice.Id);
                    break;
            }

            return invoiceLines;
        }

        /// <summary>
        /// Gets collection of InvoiceLineViewModels
        /// </summary>
        /// <returns>collection with InvoiceLineViewModels</returns>
        public IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModels(string invoiceId)
        {
            var invoiceLines = GetInvoiceLines(invoiceId, IncludeLevel.Level2);

            var models = invoiceLines
                .Select(x => new InvoiceLineViewModel(x));

            return models;
        }

        /// <summary>
        /// Gets Invoice line for given Id or throws exceptions if budget not found.
        /// </summary>
        /// <param name="lineId">Invoice line id</param>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>Budget based on budget for given Id</returns>
        public InvoiceLine GetInvoiceLineById(string lineId, string invoiceId, IncludeLevel includeLevel)
        {
            var invoice = GetInvoiceById(invoiceId, IncludeLevel.None);

            InvoiceLine invoiceLine;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoiceLine = Context.InvoiceLines
                        .SingleOrDefault(x => x.Id == lineId && x.InvoiceId == invoice.Id);
                    break;

                case IncludeLevel.Level1:
                    invoiceLine = Context.InvoiceLines
                        .Include(x => x.Invoice)
                        .Include(x => x.Budget)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .SingleOrDefault(x => x.Id == lineId && x.InvoiceId == invoice.Id);
                    break;

                default:
                    invoiceLine = Context.InvoiceLines
                        .Include(x => x.Invoice).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Invoice).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Invoice).ThenInclude(x => x.Budget)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .SingleOrDefault(x => x.Id == lineId && x.InvoiceId == invoice.Id);
                    break;
            }

            if (invoiceLine == null)
            {
                throw new ArgumentException("Linia o podanym Id nie istnieje dla wskazanej faktury", nameof(invoice));
            }

            return invoiceLine;
        }

        /// <summary>
        /// Add Invoice line based on given InvoiceLineViewModel.
        /// </summary>
        /// <param name="model">InvoiceLineViewModel</param>
        /// <param name="createdBy">ApplicationUser that creates invoice line</param>
        /// <param name="recalculateInvoiceValues">Indicates if invoice line values should be recalculated based on price, tax rate, currency rate</param>
        /// <param name="recalculateBudgetValues">indicates if recalculation of budget invoiced amount should to be done</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        public InvoiceLineReturnResult AddLine(InvoiceLineViewModel model, ApplicationUser createdBy, bool recalculateInvoiceValues, bool recalculateBudgetValues)
        {
            if (model == null || createdBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var invoice = GetInvoiceById(model.InvoiceId, IncludeLevel.None);

            if (invoice == null)
            {
                throw new ArgumentException($"Faktura o podanym ID nie istnieje.", nameof(invoice));
            }

            if (invoice.Status == Status.Cancelled)
            {
                throw new InvalidOperationException("Fatura jest anulowana. Nie można dodawać nowych linii do anulowanej faktury");
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

            return new InvoiceLineReturnResult()
            {
                Id = newInvoiceLine.Id,
                InvoiceId = newInvoiceLine.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                LineNumber = newInvoiceLine.LineNumber,
                Status = newInvoiceLine.Status.ToString()
            };
        }

        /// <summary>
        /// Modify Invoice line based on given InvoiceLineViewModel or throws exceptions if invoice not found.
        /// </summary>
        /// <param name="model">InvoiceLineViewModel</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice line</param>
        /// <param name="recalculateInvoiceValues">Indicates if invoice line values should be recalculated based on price, tax rate, currency rate</param>
        /// <param name="recalculateBudgetValues">indicates if recalculation of budget invoiced amount should to be done</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        public InvoiceLineReturnResult EditLine(InvoiceLineViewModel model, ApplicationUser modifiedBy, bool recalculateInvoiceValues, bool recalculateBudgetValues)
        {
            if (model == null || modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(model), "Nieprawidłowe parametry");
            }

            var invoiceLine = GetInvoiceLineById(model.Id, model.InvoiceId, IncludeLevel.Level1);

            if (invoiceLine.Status == Status.Cancelled)
            {
                throw new InvalidOperationException("Linia fatury jest anulowana. Nie można edytować anulowanej linii faktury");
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
                InvoiceNumber = invoiceLine.Invoice.InvoiceNumber,
                LineNumber = invoiceLine.LineNumber,
                Status = invoiceLine.Status.ToString()
            };
        }

        /// <summary>
        /// Changes status for given invoice line
        /// </summary>
        /// <param name="lineId">Invoice line id</param>
        /// <param name="invoiceId">invoice id</param>
        /// <param name="newStatus">new status</param>
        /// <param name="modifiedBy">ApplicationUser that is modifying invoice line</param>
        /// <returns>InvoiceLineReturnResult with line id, invoice id, line number and status</returns>
        public InvoiceLineReturnResult ChangeLineStatus(string lineId, string invoiceId, Status newStatus, ApplicationUser modifiedBy)
        {
            var invoiceLine = GetInvoiceLineById(lineId, invoiceId, IncludeLevel.None);

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

            return new InvoiceLineReturnResult()
            {
                Id = invoiceLine.Id,
                InvoiceId = invoiceLine.InvoiceId,
                LineNumber = invoiceLine.LineNumber,
                Status = invoiceLine.Status.ToString()
            };
        }

        /// <summary>
        /// Gets next invoice line number for given invoice id
        /// </summary>
        /// <param name="invoiceId">invoice id</param>
        /// <returns>next invoice line number for given invoice id</returns>
        public int GetNextInvoiceLineNum(string invoiceId)
        {
            var invoiceLines = Context.InvoiceLines.Where(x => x.InvoiceId == invoiceId);

            var lineNum = !invoiceLines.Any() ? 0 : invoiceLines.Max(x => x.LineNumber);

            return lineNum + 1;
        }

        /// <summary>
        /// Gets collection of SelectListItem for select field in View with opened customers
        /// </summary>
        /// <param name="selectedCustomer">Customer which will be selected</param>
        /// <returns>collection of SelectListItem for select field in View with opened customers</returns>
        public IEnumerable<SelectListItem> GetOpenCustomersItemList(CustomerViewModel selectedCustomer = null)
        {
            var customersItemList = CustomerManager.GetCustomers(IncludeLevel.None)
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

        /// <summary>
        /// Gets collection of SelectListItem for select field in View with opened budgets
        /// </summary>
        /// <param name="selectedBudget">Budget which will be selected</param>
        /// <returns>collection of SelectListItem for select field in View with opened budgets</returns>
        public IEnumerable<SelectListItem> GetOpenBudgetsItemList(BudgetViewModel selectedBudget = null)
        {
            var budgetsItemList = BudgetManager.GetBudgets(IncludeLevel.None)
                .Where(x => x.Status == Status.Opened)
                .OrderByDescending(x => x.CreatedDate)
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

            return budgetsItemList;
        }

        /// <summary>
        /// Gets collection of Invoice models for given customer id
        /// </summary>
        /// <param name="customerId">customer Id for which invoices should be retrieved</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection of invoices for given customer id</returns>
        public IEnumerable<Invoice> GetInvoicesForCustomer(string customerId, IncludeLevel includeLevel)
        {
            IEnumerable<Invoice> invoices;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoices = Context.Invoices
                        .Where(x => x.CustomerId == customerId);
                    break;

                case IncludeLevel.Level1:
                    invoices = Context.Invoices
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .Include(x => x.Budget)
                        .Include(x => x.Customer)
                        .Where(x => x.CustomerId == customerId);
                    break;

                default:
                    invoices = Context.Invoices
                        .Include(x => x.CreatedBy).ThenInclude(x => x.Manager)
                        .Include(x => x.LastModifiedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.LastModifiedBy)
                        .Include(x => x.Customer).ThenInclude(x => x.CreatedBy)
                        .Where(x => x.CustomerId == customerId);
                    break;
            }

            return invoices;
        }

        /// <summary>
        /// Gets collection of InvoiceView models for given customer id
        /// </summary>
        /// <param name="customerId">customer Id for which invoices should be retrieved</param>
        /// <returns>collection of InvoiceView models for given customer id</returns>
        public IEnumerable<InvoiceViewModel> GetInvoiceViewModelsForCustomer(string customerId)
        {
            var models = GetInvoicesForCustomer(customerId, IncludeLevel.Level2)
                .Select(x => new InvoiceViewModel(x));

            return models;
        }

        /// <summary>
        /// Gets collection of Invoice line models for given budget id
        /// </summary>
        /// <param name="budgetId">budget Id for which invoice liens should be retrieved</param>
        /// <param name="includeLevel">indicates level of dependencies to be retrieved from database</param>
        /// <returns>collection of invoices for given customer</returns>
        public IEnumerable<InvoiceLine> GetInvoiceLinesForBudget(string budgetId, IncludeLevel includeLevel)
        {
            IEnumerable<InvoiceLine> invoicesLines;

            switch (includeLevel)
            {
                case IncludeLevel.None:
                    invoicesLines = Context.InvoiceLines
                        .Where(x => x.BudgetId == budgetId);
                    break;

                case IncludeLevel.Level1:
                    invoicesLines = Context.InvoiceLines
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .Include(x => x.Invoice)
                        .Include(x => x.Budget)
                        .Where(x => x.BudgetId == budgetId);
                    break;

                default:
                    invoicesLines = Context.InvoiceLines
                        .Include(x => x.CreatedBy)
                        .Include(x => x.LastModifiedBy)
                        .Include(x => x.Invoice).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Invoice).ThenInclude(x => x.Customer).ThenInclude(x => x.CreatedBy)
                        .Include(x => x.Budget).ThenInclude(x => x.CreatedBy)
                        .Where(x => x.BudgetId == budgetId);
                    break;
            }

            return invoicesLines;
        }

        /// <summary>
        /// Gets collection of InvoiceLineView models for given customer id
        /// </summary>
        /// <param name="budgetId">budget Id for which invoice lines should be retrieved</param>
        /// <returns>collection of InvoiceLineView models for given customer id</returns>
        public IEnumerable<InvoiceLineViewModel> GetInvoiceLineViewModelsForBudget(string budgetId)
        {
            var models = GetInvoiceLinesForBudget(budgetId, IncludeLevel.Level3)
                .Select(x => new InvoiceLineViewModel(x));

            return models;
        }
    }
}