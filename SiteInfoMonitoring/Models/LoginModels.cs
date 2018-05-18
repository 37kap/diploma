using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class LoginModels
    {
        public class LoginModel
        {
            [Required]
            [Display(Name = "Логин")]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }
        }

        public class RegisterModel
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }

            [Required]
            public int Age { get; set; }
        }
    }
}