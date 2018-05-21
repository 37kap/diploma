using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Models;
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
                EduSiteParser htmlParser;
                List<Division> divs;
                if (IsAdminUser())
                {
                    siteChecker = new SiteChecker(name);
                    divs = siteChecker.CheckDivisionsExist();
                    htmlParser = new EduSiteParser(name, divs, siteChecker.XmlParser.GetUsers());
                }
                else
                {
                    siteChecker = new SiteChecker(name, User.Identity.Name);
                    divs = siteChecker.CheckDivisionsExist();
                    htmlParser = new EduSiteParser(name, divs, siteChecker.XmlParser.GetUserByName(User.Identity.Name));
                }
                ViewBag.SiteAvailability = "Сайт " + name + (siteChecker.CheckSiteAvailability() ? " доступен" : " недоступен");                
                if (siteChecker.XmlParser.Exception != null)
                {
                    ViewBag.Exception = siteChecker.XmlParser.Exception;
                }
                htmlParser.StartParse();
                return View(divs);
            }
            else
            {
                List<Division> divs = null;
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
