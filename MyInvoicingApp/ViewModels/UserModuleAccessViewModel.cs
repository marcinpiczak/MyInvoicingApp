using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class UserModuleAccessViewModel : ModuleAccessViewModel
    {
        public ApplicationUser User { get; set; }

        public UserModuleAccessViewModel()
        {
        }

        public UserModuleAccessViewModel(UserModuleAccess model) : base(model)
        {
            AccessorType = AccessorType.User;
            User = model.User;
        }
    }
}