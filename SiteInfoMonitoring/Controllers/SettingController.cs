using Calabonga.Portal.Config;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class SettingController : Controller
    {
        private readonly IConfigService<CurrentAppSettings> _configService;

        public SettingController(IConfigService<CurrentAppSettings> configService)
        {
            SettingsManager.Settings = configService.Config;
            configService.Reload();
            _configService = configService;
        }

        [NoCache]
        public ActionResult Index()
        {
            if (IsAdminUser())
            {
                var settings = _configService.Config;
                return View(settings);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        public ActionResult Index(CurrentAppSettings settings)
        {
            if (IsAdminUser())
            {
                _configService.SaveChanges(settings);
                SettingsManager.Settings = settings;
                SiteAnalysisSheduler.Stop();
                if (SettingsManager.Settings.AutoAnalysis && SettingsManager.Settings.DateAutoAnalysis > 0)
                {
                    new Thread(t => SiteAnalysisSheduler.Start()).Start();
                }
                ViewBag.ResultOfSave = "Настройки успешно сохранены.";
                return View(settings);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        public bool IsAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = new XmlParser().GetUsers().FirstOrDefault(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    if (user.Role == Core.Enums.RolesEnum.admin)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public CurrentAppSettings GetSettings()
        {
            return _configService.Config;
        }
    }
}