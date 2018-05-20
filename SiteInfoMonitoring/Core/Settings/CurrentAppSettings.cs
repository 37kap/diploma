using Calabonga.Portal.Config;
using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Core.Settings
{
    public class CurrentAppSettings : AppSettings
    {
        [Display(Name = "Адрес сайта по умолчанию")]
        public string DefaultSiteAddress { get; set; }

        [Display(Name = "Электронная почта для отправки сообщений пользователям")]
        public string SmtpLogin { get; set; }

        [Display(Name = "Пароль от электронной почты")]
        public string SmtpPassword { get; set; }
        
        [Display(Name = "SMTP Хост")]
        public string SmtpHost { get; set; }

        [Display(Name = "Имя XML-файла")]
        public string XmlFile { get; set; }
    }
}