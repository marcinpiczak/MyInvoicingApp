using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Nazwa musi zostać uzupełniona")]
        [MaxLength(100, ErrorMessage = "Nazwa może mieć maksymalnie {1} znaków")]
        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis musi zostać uzupełniony")]
        [MaxLength(255, ErrorMessage = "Nazwa może mieć maksymalnie {1} znaków")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Opis")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Miasto musi zostać uzupełniony")]
        [MaxLength(100, ErrorMessage = "Miasto może mieć maksymalnie {1} znaków")]
        [DisplayName("Miasto")]
        public string City { get; set; }

        [Required(ErrorMessage = "Kod pocztowy musi zostać uzupełniony")]
        [MaxLength(10, ErrorMessage = "Kod pocztwoy może mieć maksymalnie {1} znaków")]
        [DataType(DataType.PostalCode)]
        [DisplayName("Kod pocztowy")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Ulica musi zostać uzupełniona")]
        [MaxLength(100, ErrorMessage = "Ulica może mieć maksymalnie {1} znaków")]
        [DisplayName("Ulica")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Nr budynku musi zostać uzupełniony")]
        [MaxLength(10, ErrorMessage = "Nr budynku może mieć maksymalnie {1} znaków")]
        [DisplayName("Nr budynku")]
        public string BuildingNumber { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Notatki")]
        public string Notes { get; set; }

        [MaxLength(15, ErrorMessage = "Nr budynku może mieć maksymalnie {1} znaków")]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Nr telefonu")]
        public string PhoneNumber { get; set; }

        [MaxLength(50, ErrorMessage = "Domyślna metoda płatności może mieć maksymalnie {1} znaków")]
        [DisplayName("Domyślna metoda płatności")]
        public string DefaultPaymentMethod { get; set; }

        public AccessViewModel Accesses { get; set; } = new AccessViewModel();

        public ICollection<InvoiceViewModel> Invoices { get; set; }
        
        public CustomerViewModel()
        {
        }

        public CustomerViewModel(Customer model)
        {
            if (model?.CreatedBy == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Id = model.Id;
            Status = model.Status;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            LastModifiedBy = model.LastModifiedBy;
            LastModifiedDate = model.LastModifiedDate;
            Name = model.Name;
            Description = model.Description;
            City = model.City;
            PostalCode = model.PostalCode;
            Street = model.Street;
            BuildingNumber = model.BuildingNumber;
            Notes = model.Notes;
            PhoneNumber = model.PhoneNumber;
            DefaultPaymentMethod = model.DefaultPaymentMethod;

        }
    }
}