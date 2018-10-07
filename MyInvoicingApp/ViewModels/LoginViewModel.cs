using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login jest wymagany")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [DisplayName("Hasło")]
        public string Password { get; set; }

        [DisplayName("Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}
