using Microsoft.AspNetCore.Identity;

namespace MyInvoicingApp.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }

        public Status Status { get; set; } = Status.Opened;

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public ApplicationRole()
        {
        }
    }
}
