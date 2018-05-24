using SiteInfoMonitoring.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class Itemprop
    {
        [Display(Name = "Значение")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Value { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Description { get; set; }       

        [Display(Name = "Ответственный пользователь")]
        [Required(ErrorMessage = "Заполните поле")]
        public User ResponsibleUser { get; set; }

        [Display(Name = "Тип")]
        [Required(ErrorMessage = "Заполните поле")]
        public ItempropTypeEnum Type { get; set; }

        public bool IsExist = false;

        public int Count = 0;

        public Itemprop()
        {
            Type = ItempropTypeEnum.Required;
        }
    }
}