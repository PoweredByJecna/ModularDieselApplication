using System.ComponentModel.DataAnnotations;
using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Domain.Entities
{
   // Tento model slouží jako view model pro přihlašovací stránku
    public class LoginViewModel
    {
        public LoginInputModel Input { get; set; } = new LoginInputModel();
    }

    // Model obsahující přihlašovací údaje
    public class LoginInputModel
    {
        [Required(ErrorMessage = "Uživatelské jméno je povinné")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Heslo je povinné")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}