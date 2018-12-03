using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class RoleModuleAccessViewModel : ModuleAccessViewModel
    {
        public ApplicationRole Role { get; set; }

        public RoleModuleAccessViewModel()
        {
        }

        public RoleModuleAccessViewModel(RoleModuleAccess model) : base(model)
        {
            AccessorType = AccessorType.Role;
            Role = model.Role;
        }
    }
}