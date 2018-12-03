using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class RoleModuleAccess : ModuleAccess
    {
        [ForeignKey("AccessorId")]
        public ApplicationRole Role { get; set; }

        public RoleModuleAccess()
        {
            Id = Guid.NewGuid().ToString();
            AccessorType = AccessorType.Role;
        }
    }
}