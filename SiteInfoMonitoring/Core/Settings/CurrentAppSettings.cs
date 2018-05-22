using Calabonga.Portal.Config;
using System;
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

        [Display(Name = "Имя XML-файла с описанием страниц обязательного раздела")]
        public string XmlFileDivisions { get; set; }

        [Display(Name = "Имя XML-файла с пользователями")]
        public string XmlFileUsers { get; set; }

        [Display(Name = "Отправлять письмо с результатом после проверки в ручном режиме")]
        public bool SendEmails { get; set; }

        [Display(Name = "Отправлять администраторам письмо с результатом после проверки в автоматическом режиме")]
        public bool AutoSendEmailsToAdmin { get; set; }

        [Display(Name = "Автоматическая проверка")]
        public bool AutoAnalysis { get; set; }

        [Display(Name = "Как часто проводить автоматическую проверку? Каждые X дней")]
        public int DateAutoAnalysis { get; set; }
    }
}