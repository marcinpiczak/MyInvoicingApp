using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class BudgetViewModel : BaseViewModel
    {
        [Required]
        [MaxLength(20)]
        [DisplayName("Kod budżetu")]
        [ReadOnly(true)]
        public string BudgetNumber { get; set; }

        [Required(ErrorMessage = "Opis budżetu jest wymagany")]
        [DisplayName("Opis")]
        [MaxLength(255)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kwota budżetu musi być uzupełniona")]
        [DisplayName("Kwota budżetu")]
        [Range(0, Double.MaxValue, ConvertValueInInvariantCulture = true, ErrorMessage = "Kwota budżetu musi być większa od zera")]
        [DataType(DataType.Currency)]
        public decimal CommitedAmount { get; set; }

        [DisplayName("Kwota faktur")]
        [DataType(DataType.Currency)]
        public decimal InvoicedAmount { get; set; }

        [DisplayName("Właściciel")]
        public ApplicationUser Owner { get; set; }

        public ICollection<InvoiceLineViewModel> InvoiceLines { get; set; }

        public BudgetViewModel()
        {
        }

        public BudgetViewModel(Budget model)
        {
            if (model?.CreatedBy == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Id = model.Id;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            LastModifiedBy = model.LastModifiedBy;
            LastModifiedDate = model.LastModifiedDate;
            Status = model.Status;
            BudgetNumber = model.BudgetNumber;
            Description = model.Description;
            CommitedAmount = model.CommitedAmount;
            InvoicedAmount = model.InvoicedAmount;
            Owner = model.Owner;

        }
    }
}