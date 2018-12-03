using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Budget : BaseTable
    {
        [Required]
        [MaxLength(20)]
        public string BudgetNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public decimal CommitedAmount { get; set; }

        public decimal InvoicedAmount { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        public ICollection<InvoiceLine> InvoiceLines { get; set; }

        public Budget()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
