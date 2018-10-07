using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class RoleViewModel
    {
        [HiddenInput]
        public string Id { get; set; }

        [Required(ErrorMessage = "Nazwa stanowiska jest wymagana")]
        [MinLength(5, ErrorMessage = "Nazwa stanowiska musi się składać przynajmniej z {1} znaków")]
        [DisplayName("Nazwa stanowiska")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Opis stanowiska jest wymagany")]
        [MinLength(5, ErrorMessage = "Opis stanowiska musi się składać przynajmniej z {1} znaków")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Opis stanowiska")]
        public string Description { get; set; }

        public IEnumerable<ApplicationUser> AsignedUsers { get; set; }

        public RoleViewModel(ApplicationRole model)
        {
            if (model == null)
            {
                throw new ArgumentNullException();
            }

            Id = model.Id;
            Position = model.Name;
            Description = model.Description;
        }

        public RoleViewModel()
        {
        }
    }
}
