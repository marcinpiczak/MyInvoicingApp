using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class BaseTable
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedById { get; set; }

        [ForeignKey("LastModifiedById")]
        public ApplicationUser LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Status Status { get; set; } = Status.Opened;
    }
}