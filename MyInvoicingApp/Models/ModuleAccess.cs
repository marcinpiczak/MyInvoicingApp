using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class ModuleAccess : BaseTable
    {
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public ApplicationRole Role { get; set; }

        public string Module { get; set; }

        public string Action { get; set; }

        public ModuleAccess()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}