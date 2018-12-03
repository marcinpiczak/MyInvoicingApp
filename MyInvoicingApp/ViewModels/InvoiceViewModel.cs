using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class InvoiceViewModel : BaseViewModel
    {
        [Required]
        [MaxLength(20)]
        [DisplayName("Numer faktury")]
        [ReadOnly(true)]
        public string InvoiceNumber { get; set; }

        [MaxLength(50, ErrorMessage = "Numer referencyjny może się składać maksymalnie {1} znaków")]
        [DisplayName("Numer referencyjny")]
        public string ReferenceNumber { get; set; }

        [Required(ErrorMessage = "Metoda płatności musi zostać uzupełniona")]
        [MaxLength(50, ErrorMessage = "Metoda płatności może się składać maksymalnie {1} znaków")]
        [DisplayName("Metoda płatności")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Data płatności musi zostać uzupełniona")]
        [DisplayName("Data płatności")]
        [DataType(DataType.Date)]
        public DateTime PaymentDueDate { get; set; }

        [DisplayName("Data wydania")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [DisplayName("Data przyjęcia")]
        [DataType(DataType.Date)]
        public DateTime ReceiveDate { get; set; }

        [Required(ErrorMessage = "Kod klienta musi zostać wybrany")]
        public string CustomerId { get; set; }

        [DisplayName("Klient")]
        public CustomerViewModel Customer { get; set; }

        [Required(ErrorMessage = "Waluta faktury musi zostać podana")]
        [MaxLength(5, ErrorMessage = "Kod waluty może się składać maksymalnie z {1} znaków")]
        [DisplayName("Waluta")]
        public string Currency { get; set; }

        public string BudgetId { get; set; }

        [DisplayName("Domyślny budżet")]
        public BudgetViewModel Budget { get; set; }

        [DisplayName("Właściciel")]
        public ApplicationUser Owner { get; set; }

        public IEnumerable<SelectListItem> CustomerItemList { get; set; }

        public IEnumerable<SelectListItem> BudgetItemList { get; set; }

        public InvoiceLineViewModel InvoiceLine { get; set; }

        public IEnumerable<InvoiceLineViewModel> InvoiceLines { get; set; }

        public IEnumerable<AttachmentViewModel> Attachments { get; set; }

        public InvoiceViewModel()
        {
        }

        public InvoiceViewModel(Invoice model)
        {
            if (model.CreatedBy == null || model?.Customer == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Id = model.Id;
            Status = model.Status;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            LastModifiedBy = model.LastModifiedBy;
            LastModifiedDate = model.LastModifiedDate;
            InvoiceNumber = model.InvoiceNumber;
            ReferenceNumber = model.ReferenceNumber;
            PaymentMethod = model.PaymentMethod;
            PaymentDueDate = model.PaymentDueDate;
            IssueDate = model.IssueDate;
            ReceiveDate = model.ReceiveDate;
            Customer = new CustomerViewModel(model.Customer);
            CustomerId = model.CustomerId;
            Currency = model.Currency;
            Budget = model.Budget == null ? new BudgetViewModel() : new BudgetViewModel(model.Budget);
            BudgetId = model.BudgetId;
            Owner = model.Owner;
        }
    }
}