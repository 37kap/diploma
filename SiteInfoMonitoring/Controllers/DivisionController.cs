using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using System.Collections.Generic;
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
                var siteChecker = new SiteChecker(name);
                ViewBag.SiteAvailability = "Сайт " + name + (siteChecker.CheckSiteAvailability() ? " доступен" : " недоступен");
                var divs = siteChecker.CheckDivisionsExist();
                if (siteChecker.XmlParser.Exception != null)
                {
                    ViewBag.Exception = siteChecker.XmlParser.Exception;
                }
                var htmlParser = new EduSiteParser(name, divs, siteChecker.XmlParser.GetUsers());
                //htmlParser.StartParse();
                return View(divs);
            }
            else
            {
                var divs = new List<Models.Division>();
                return View(divs);
            }
        }
    }
}
