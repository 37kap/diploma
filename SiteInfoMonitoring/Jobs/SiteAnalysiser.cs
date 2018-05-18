using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SiteInfoMonitoring.Jobs
{
    public class SiteAnalysiser : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var message = new MailMessage("noreplyisuct@gmail.com", "kap120796@gmail.com"))
            {
                message.Subject = "Результат проверки обязательного раздела сайта образовательной организации";
                //TODO: change body of email of job
                message.Body = "Message body test at " + DateTime.Now;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("noreplyisuct@gmail.com", "sqaynmbmuhojcgrh") //isuct123
                })
                {
                    await client.SendMailAsync(message);
                }
            }
        }
}