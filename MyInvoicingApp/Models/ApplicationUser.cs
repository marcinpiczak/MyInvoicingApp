using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MyInvoicingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Status Status { get; set; } = Status.Opened;

        public string ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public ApplicationUser Manager { get; set; }

        //public ICollection<Invoice> CreatedInvoices { get; set; }

        //public ICollection<Invoice> LastModifiedInvoices { get; set; }

        //public ICollection<InvoiceLine> CreatedInvoiceLines { get; set; }

        //public ICollection<InvoiceLine> LastModifiedInvoiceLines { get; set; }

        public ICollection<Budget> CreatedBudgets { get; set; }

        public ICollection<Budget> LastModifiedBudgets { get; set; }

        public ICollection<Budget> OwnedBudgets { get; set; }

        public ICollection<Customer> CreatedCustomers { get; set; }

        public ICollection<Customer> LastModifiedCustomers { get; set; }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        public ApplicationUser()
        {
        }
    }
}
