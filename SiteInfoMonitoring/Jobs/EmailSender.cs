using Quartz;
using SiteInfoMonitoring.Controllers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SiteInfoMonitoring.Jobs
{
    public static class EmailSender
    {
        public static async void Send(string email, string messageBody)
        {
            using (var message = new MailMessage(SettingsManager.Settings.SmtpLogin, email))
            {
                message.Subject = "Результат проверки обязательного раздела сайта образовательной организации";
                message.Body = messageBody;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = SettingsManager.Settings.SmtpHost,
                    Port = 587,
                    Credentials = new NetworkCredential(SettingsManager.Settings.SmtpLogin, SettingsManager.Settings.SmtpPassword) //Pass: sqaynmbmuhojcgrh || isuct123
                })
                {
                    await client.SendMailAsync(message);
                }
            }
        }

    }
}