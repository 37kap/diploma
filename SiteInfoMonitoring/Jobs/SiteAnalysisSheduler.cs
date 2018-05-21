using Quartz;
using Quartz.Impl;
using SiteInfoMonitoring.Core.Settings;
using System.Threading.Tasks;

namespace SiteInfoMonitoring.Jobs
{
    public class SiteAnalysisSheduler
    {
        private static IJobDetail job;
        private static IScheduler scheduler;
        public static async void Start()
        {
            if (SettingsManager.Settings.AutoAnalysis && SettingsManager.Settings.DateAutoAnalysis > 0)
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                job = JobBuilder.Create<SiteAnalysiser>().Build();

                ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("AutoSiteAnalysisTrigger", "AutoSiteAnalysisGroup")     // идентифицируем триггер с именем и группой
                    .StartNow()                            // запуск сразу после начала выполнения
                    .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                        .WithIntervalInHours(24 * SettingsManager.Settings.DateAutoAnalysis)          // каждые Х минут
                        .RepeatForever())                   // бесконечное повторение
                    .Build();                               // создаем триггер
                await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
            }
        }
        private static async void StopJob()
        {
            if (job != null)
            {
                await scheduler.DeleteJob(job.Key);
            }
        }
        public static void Stop()
        {
            new Task(StopJob).Start();
        }
    }
}