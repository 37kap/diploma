using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using SiteInfoMonitoring.Models;
using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Jobs;
using System.Threading;

namespace SiteInfoMonitoring.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            users = new XmlParser().LoadUsers();
            return View();
        }

        private static List<User> users = new List<Models.User>();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginModels.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;

                user = users.FirstOrDefault(u => u.Login == model.Name && u.Password == model.Password);
                
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);

                    return RedirectToAction("Analysis", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}