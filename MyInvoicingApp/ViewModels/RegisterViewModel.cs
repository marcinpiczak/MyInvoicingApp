using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Login jest wymagany")]
        [MinLength(5, ErrorMessage = "Login musi się składać przynajmniej {1} znaków")]
        public string Login { get; set; }

        [Required(ErrorMessage = "E-mail jest wymagany")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "Podaj prawidłowy e-mail")]
        [EmailAddress(ErrorMessage = "Podaj prawidłowy e-mail")]
        [DisplayName("E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [MinLength(3, ErrorMessage = "Imię musi się składać przynajmniej {1} znaków")]
        [DisplayName("Imię")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [MinLength(3, ErrorMessage = "Nazwisko musi się składać przynajmniej {1} znaków")]
        [DisplayName("Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [DisplayName("Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [DisplayName("Powtórz hasło")]
        [Compare("Password", ErrorMessage = "Hasła muszą być takie same")]
        public string RepeatPassword { get; set; }
    }
}
