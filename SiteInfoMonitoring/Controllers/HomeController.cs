using Calabonga.Portal.Config;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using System.Linq;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigService<CurrentAppSettings> _configService;

        public HomeController(IConfigService<CurrentAppSettings> configService)
        {
            SettingsManager.Settings = configService.Config;
            configService.Reload();
            _configService = configService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Analysis()
        {
            ViewBag.SiteName = SettingsManager.Settings.DefaultSiteAddress;
            return View();
        }

        public ActionResult UserLogin()
        {
            ViewBag.IsAdminUser = IsAdminUser();
            return PartialView(System.Web.HttpContext.Current.User);
        }

        public bool IsAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = new XmlParser().LoadUsers().FirstOrDefault(u => u.Login == User.Identity.Name);
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
    }
}