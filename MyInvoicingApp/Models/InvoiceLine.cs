using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class InvoiceLine : BaseTable
    {
        [Required]
        public string InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        public int LineNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [Required]
        public decimal TaxRate { get; set; }

        public decimal Netto { get; set; }

        public decimal Tax { get; set; }

        public decimal Gross { get; set; }

        [Required]
        [MaxLength(5)]
        public string Currency { get; set; }

        public decimal CurrencyRate { get; set; }

        public decimal BaseNetto { get; set; }

        public decimal BaseTax { get; set; }

        public decimal BaseGross { get; set; }

        [Required]
        public string BudgetId { get; set; }

        [ForeignKey("BudgetId")]
        public Budget Budget { get; set; }

        public InvoiceLine()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
