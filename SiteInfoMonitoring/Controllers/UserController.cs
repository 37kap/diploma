using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class UserController : Controller
    {
        private List<User> users = new List<User>();
        [Authorize]
        public ActionResult Index(bool userAdded = false)
        {            
            if (IsAdminUser())
            {
                ViewBag.CurrentUser = User.Identity.Name;
                users = new XmlParser().LoadUsers();
                if (userAdded)
                {
                    userAdded = false;
                    ViewBag.ResultOfSave = "Пользователь добавлен в базу.";
                }
                return View(users);
            }
            else
            {
                return Redirect("/Home/Index");
            }            
        }

        [Authorize]
        public ActionResult Rerole(string userLogin)
        {
            if (IsAdminUser() && userLogin != null)
            {
                var xml = new XmlParser();
                var users = xml.LoadUsers();
                var User = users.FirstOrDefault(u => u.Login == userLogin);
                if (User.Role == Core.Enums.RolesEnum.admin)
                {
                    User.Role = Core.Enums.RolesEnum.user;
                }
                else
                {
                    User.Role = Core.Enums.RolesEnum.admin;
                }
                xml.SaveUsers(users);
                return Redirect("/User/Index");
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult Delete(string userLogin)
        {
            if (IsAdminUser() && userLogin != null)
            {
                var xml = new XmlParser();
                var users = xml.LoadUsers();
                var User = users.RemoveAll(u => u.Login == userLogin);                
                xml.SaveUsers(users);
                return Redirect("/User/Index");
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        public ActionResult Add()
        {
            if (IsAdminUser())
            {
                return View();
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        public ActionResult Add(User user)
        {
            if (IsAdminUser())
            {
                try
                {
                    var xml = new XmlParser();
                    var users = xml.LoadUsers();
                    users.Add(user);
                    xml.SaveUsers(users);
                    return Redirect("/User/Index?userAdded=true");
                }
                catch (Exception ex)
                {
                    ViewBag.ResultOfSave = "Произошла ошибка: \n" + ex.Message;
                }
                return View(user);
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