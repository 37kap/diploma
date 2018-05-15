using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class DivisionController : Controller
    {
        // GET: Division
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Check(string name)
        {
            if (name != null)
            {
                var siteChecker = new SiteChecker(name);
                ViewBag.SiteAvailability = "Сайт " + name + (siteChecker.CheckSiteAvailability() ? " доступен" : " недоступен");
                var divs = siteChecker.GetContentFromXml();
                if (siteChecker.XmlParseException == null)
                {
                    siteChecker.CheckDivisionsExist();
                }
                else
                {
                    ViewBag.Exception = siteChecker.XmlParseException;
                }
                var htmlParser = new EduSiteParser(name, divs, siteChecker.Users);
                htmlParser.StartParse();
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
