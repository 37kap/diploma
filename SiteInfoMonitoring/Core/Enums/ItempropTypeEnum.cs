using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Core.Enums
{
    public enum ItempropTypeEnum
    {
        [Display(Name = "Обязательный")]
        Required = 0,

        [Display(Name = "Необязательный")]
        Optional = 1,

        [Display(Name = "Выборочный")]
        Selective = 2
    }
}