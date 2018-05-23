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
        [Authorize]
        public ActionResult Index(bool divisionAdded = false)
        {
            if (IsAdminUser())
            {
                List<Division> divisions = new XmlParser(SettingsManager.Settings.DefaultSiteAddress).LoadDivisions();
                if (divisionAdded)
                {
                    divisionAdded = false;
                    ViewBag.ResultOfSave = "Страница добавлена в базу.";
                }
                return View(divisions);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
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

        [Authorize]
        public ActionResult AddUser(int id)
        {
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                var users = xml.LoadUsers();
                var div = xml.LoadDivisions().Where(d => d.Id == id).First();
                ViewBag.Users = new SelectList(users.Select(u => u.Login));
                return View(div);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddUser(Division division, FormCollection form)
        {
            if (IsAdminUser())
            {
                string userLogin = form["user"].ToString();
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                var us = xml.LoadUsers().First(u => u.Login == userLogin);
                var divs = xml.LoadDivisions();
                var div = divs.First(d => d.Id == division.Id);
                div.ResponsibleUser = us;
                xml.SaveDivisions(divs);
                return View("Index", divs);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(Division division)
        {
            if (IsAdminUser())
            {
                try
                {
                    division.Url = new Uri(SettingsManager.Settings.DefaultSiteAddress + "/" + division.Url);
                    var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                    List<Division> divisions = xml.LoadDivisions();
                    divisions.Add(division);
                    xml.SaveDivisions(divisions);
                    return Redirect("/Division/Index?divisionAdded=true");
                }
                catch (Exception ex)
                {
                    ViewBag.ResultOfSave = "Произошла ошибка: \n" + ex.Message;
                }
                return View(division);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && id > 0 && divisions.Any(d => d.Id == id))
            {
                divisions.RemoveAll(d => d.Id == id);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/Index");
            }
            else
            {
                return Redirect("/Home/Index");
            }
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
                    if (siteChecker.XmlException != null && siteChecker.XmlException.InnerException != null && siteChecker.XmlException.InnerException.Message == "Отсутствует корневой элемент.")
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

        [Authorize]
        public ActionResult Edit(int id)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && id > 0 && divisions.Any(d => d.Id == id))
            {
                var users = xml.LoadUsers();
                var div = divisions.First(d => d.Id == id);
                SelectList SelectListUsers;
                if (div.ResponsibleUser != null)
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = u.Login == div.ResponsibleUser.Login ? true : false
                    }));
                }
                else
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = false
                    }));
                }
                div.ShortUrl = div.Url.ToString().Remove(0, div.Url.ToString().IndexOf(div.Url.Host) + div.Url.Host.Length + 1);

                ViewBag.Users = SelectListUsers;
                return View(div);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Division division, FormCollection form)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            var users = xml.LoadUsers();            
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && division.Id > 0 && divisions.Any(d => d.Id == division.Id))
            {
                string userLogin = form["user"].ToString();
                var us = users.Any(u => u.Login == userLogin) ? users.First(u => u.Login == userLogin) : null;
                var div = divisions.First(d => d.Id == division.Id);
                div.Description = division.Description;
                div.ResponsibleUser = us;
                div.Url = new Uri(SettingsManager.Settings.DefaultSiteAddress + "/" + division.ShortUrl);
                SelectList SelectListUsers;
                if (div.ResponsibleUser != null)
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = u.Login == div.ResponsibleUser.Login ? true : false
                    }));
                }
                else
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = false
                    }));
                }
                ViewBag.Users = SelectListUsers;
                div.ShortUrl = div.Url.ToString().Remove(0, div.Url.ToString().IndexOf(div.Url.Host) + div.Url.Host.Length + 1);
                if (division.Description == null || division.Description == "" || division.ShortUrl == null || division.ShortUrl == "")
                {
                    ViewBag.Message = "Заполните все поля.";
                    return View(xml.LoadDivisions().First(d => d.Id == division.Id));
                }
                xml.SaveDivisions(divisions);
                ViewBag.Message = "Страница успешно изменена.";
                return View(div);
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
