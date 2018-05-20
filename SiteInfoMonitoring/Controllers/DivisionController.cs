using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class DivisionController : Controller
    {
        // GET: Division
        [Authorize(Roles ="")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Check(string name)
        {
            if (name != null)
            {
                SiteChecker siteChecker;
                if (IsAdminUser())
                {
                    siteChecker = new SiteChecker(name);                    
                }
                else
                {
                    siteChecker = new SiteChecker(name, User.Identity.Name);
                }
                ViewBag.SiteAvailability = "Сайт " + name + (siteChecker.CheckSiteAvailability() ? " доступен" : " недоступен");
                var divs = siteChecker.CheckDivisionsExist();
                if (siteChecker.XmlParser.Exception != null)
                {
                    ViewBag.Exception = siteChecker.XmlParser.Exception;
                }
                var htmlParser = new EduSiteParser(name, divs, siteChecker.XmlParser.GetUsers());
                htmlParser.StartParse();
                return View(divs);
            }
            else
            {
                List<Models.Division> divs = null;
                return View(divs);
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
    }
}
