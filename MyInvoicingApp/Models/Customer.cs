using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.Models
{
    public class Customer : BaseTable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Street { get; set; }

        [Required]
        [MaxLength(10)]
        public string BuildingNumber { get; set; }

        public string Notes { get; set; }

        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string DefaultPaymentMethod { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public Customer()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
