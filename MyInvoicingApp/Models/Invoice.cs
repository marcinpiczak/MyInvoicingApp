using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class Invoice : BaseTable
    {
        [Required]
        [MaxLength(20)]
        public string InvoiceNumber { get; set; }

        [MaxLength(50)]
        public string ReferenceNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime ReceiveDate { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        [MaxLength(5)]
        public string Currency { get; set; }

        public string BudgetId { get; set; }

        [ForeignKey("BudgetId")]
        public Budget Budget { get; set; }

        public ICollection<InvoiceLine> InvoiceLines { get; set; }

        public Invoice()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
