using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class UserModuleAccess : ModuleAccess
    {
        [ForeignKey("AccessorId")]
        public ApplicationUser User { get; set; }

        public UserModuleAccess()
        {
            Id = Guid.NewGuid().ToString();
            AccessorType = AccessorType.User;
        }
    }
}