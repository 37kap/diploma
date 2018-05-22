using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class DivisionController : Controller
    {
        // GET: Division
        [Authorize(Roles = "")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Check(string name)
        {
            if (name != null)
            {
                SiteChecker siteChecker = null;
                EduSiteParser htmlParser;
                List<Division> divs = new List<Division>();
                try
                {
                    if (IsAdminUser())
                    {
                        siteChecker = new SiteChecker(name);
                        divs = siteChecker.CheckDivisionsExist();
                        htmlParser = new EduSiteParser(name, divs, siteChecker.XmlParser.LoadUsers());
                    }
                    else
                    {
                        siteChecker = new SiteChecker(name, User.Identity.Name);
                        divs = siteChecker.CheckDivisionsExist();
                        htmlParser = new EduSiteParser(name, divs, new List<User>() { siteChecker.XmlParser.LoadUserByName(User.Identity.Name) });
                    }
                    ViewBag.SiteAvailability = "Сайт " + name + (siteChecker.CheckSiteAvailability() ? " доступен" : " недоступен");
                    if (siteChecker.XmlParser.Exception != null)
                    {
                        ViewBag.Exception = siteChecker.XmlParser.Exception;
                    }
                    htmlParser.StartParse(false, siteChecker.XmlParser.LoadUserByName(User.Identity.Name));
                }
                catch
                {
                    if (siteChecker.XmlException!= null && siteChecker.XmlException.InnerException != null && siteChecker.XmlException.InnerException.Message == "Отсутствует корневой элемент.")
                    {
                        ViewBag.Exception = "Не найден XML-файл с описанием страниц обязательного раздела по пути \"" + SettingsManager.Settings.XmlFileDivisions + "\"";
                    }
                }
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
