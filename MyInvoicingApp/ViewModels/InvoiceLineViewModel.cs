using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class InvoiceLineViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "ID faktury jest wymagane. Nie utworzono poprawnie faktury")]
        public string InvoiceId { get; set; }

        public InvoiceViewModel Invoice { get; set; }

        [DisplayName("Numer linii")]
        public int LineNumber { get; set; } = 0;

        [Required(ErrorMessage = "Kod pozycji musi zostać uzupełniony")]
        [MaxLength(50, ErrorMessage = "Kod pozycji może się składać z maksymalnie {1} znaków")]
        [DisplayName("Kod pozycji")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Opis pozycji musi zostać uzupełniony")]
        [MaxLength(255, ErrorMessage = "Kod pozycji może się składać z maksymalnie {1} znaków")]
        [DisplayName("Opis")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ilość musi zostać uzupełniona")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Ilość musi zawierać się między {1} a {2}")]
        [DisplayName("Ilość")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Cena jednostkowa musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Cena jednostkowa musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("Cena jedn.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stawka VAT musi zostać uzupełniona")]
        [Range(0, 100, ErrorMessage = "Stawka VAT musi zawierać się między {1}% a {2}%")]
        [DisplayName("Stawka Vat(%)")]
        public decimal TaxRate { get; set; } = 23.0M;

        [Required(ErrorMessage = "Wartość Netto musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość Netto musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("Netto (waluta)")]
        public decimal Netto { get; set; }

        [Required(ErrorMessage = "Wartość VAT-u musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość VAT-u musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("VAT (waluta)")]
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "Wartość Brutto musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość Brutto musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("Brutto (waluta)")]
        public decimal Gross { get; set; }

        [Required(ErrorMessage = "Waluta faktury musi zostać podana")]
        [MaxLength(5, ErrorMessage = "Kod waluty może się składać maksymalnie z {1} znaków")]
        [DisplayName("Waluta")]
        public string Currency { get; set; } = "PLN";

        [Required(ErrorMessage = "Wartość kursu waluty musi zostać uzupełniona")]
        [Range(0.000001, Double.MaxValue, ErrorMessage = "Wartość kursu waluty musi zawierać się między {1} a {2}")]
        [DisplayName("Kurs waluty")]
        public decimal CurrencyRate { get; set; } = 1;

        [Required(ErrorMessage = "Wartość Netto musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość Netto musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("Netto (PLN)")]
        public decimal BaseNetto { get; set; }

        [Required(ErrorMessage = "Wartość VAT-u musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość VAT-u musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("VAT (PLN)")]
        public decimal BaseTax { get; set; }

        [Required(ErrorMessage = "Wartość Brutto musi zostać uzupełniona")]
        [Range(0, Double.MaxValue, ErrorMessage = "Wartość Brutto musi zawierać się między {1} a {2}")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [DisplayName("Brutto (PLN)")]
        public decimal BaseGross { get; set; }

        [Required(ErrorMessage = "Kod budżetu musi zostać wybrany")]
        public string BudgetId { get; set; }

        [DisplayName("Kod budżetu")]
        public BudgetViewModel Budget { get; set; }

        public InvoiceLineViewModel()
        {
        }

        public InvoiceLineViewModel(InvoiceLine model)
        {
            if (model?.Invoice == null || model?.Budget == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Id = model.Id;
            InvoiceId = model.InvoiceId;
            Status = model.Status;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            LastModifiedBy = model.LastModifiedBy;
            LastModifiedDate = model.LastModifiedDate;
            Invoice = new InvoiceViewModel(model.Invoice);
            LineNumber = model.LineNumber;
            ItemName = model.ItemName;
            Description = model.Description;
            Quantity = model.Quantity;
            Price = model.Price;
            Currency = model.Currency;
            CurrencyRate = model.CurrencyRate;
            TaxRate = model.TaxRate;
            Netto = model.Netto;
            Tax = model.Tax;
            Gross = model.Gross;
            BaseNetto = model.BaseNetto;
            BaseTax = model.BaseTax;
            BaseGross = model.BaseGross;
            Budget = new BudgetViewModel(model.Budget);
            BudgetId = model.BudgetId;
        }
    }
}