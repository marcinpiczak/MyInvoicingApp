using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class BaseViewModel
    {
        [HiddenInput]
        public string Id { get; set; }

        [Required]
        [DefaultValue(Models.Status.Opened)]
        public Status Status { get; set; }

        [DisplayName("Utworzony przez")]
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Data utworzenia")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Ostatnio zmodyfikowany przez")]
        public ApplicationUser LastModifiedBy { get; set; }

        [DisplayName("Data ostatniej modyfikacji")]
        public DateTime? LastModifiedDate { get; set; }
    }
}