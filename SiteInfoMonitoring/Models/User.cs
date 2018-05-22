using SiteInfoMonitoring.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class User
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Name { get; set; }

        [Display(Name = "Эл. почта")]
        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Введите правильный e-mail")]
        public string Email { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Password { get; set; }

        public RolesEnum Role = RolesEnum.user;

        public List<UserProblems> Problems = new List<UserProblems> ();
    }
}