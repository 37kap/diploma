using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using SiteInfoMonitoring.Models;
using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;

namespace SiteInfoMonitoring.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var siteChecker = new SiteChecker("http://isuct.ru");
            var divs = siteChecker.GetContentFromXml();
            users = siteChecker.Users;
            return View();
        }

        private static List<User> users = new List<Models.User>();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginModels.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;

                user = users.FirstOrDefault(u => u.Login == model.Name && u.Password == model.Password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(LoginModels.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                user = users.FirstOrDefault(u => u.Email == model.Name);
                if (user == null)
                {
                    // создаем нового пользователя
                    users.Add(new User { Email = model.Name, Password = model.Password});

                    user = users.Where(u => u.Email == model.Name && u.Password == model.Password).FirstOrDefault();
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
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