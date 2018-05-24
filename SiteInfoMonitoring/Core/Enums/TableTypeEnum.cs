using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Core.Enums
{
    public enum TableTypeEnum
    {
        [Display(Name = "Обязательная")]
        Required = 0,

        [Display(Name = "Необязательная")]
        Optional = 1
    }
}