using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Enums;
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
                    int id = divisions.Last().Id + 1;
                    divisions.Add(division);
                    if (division.Description == null || division.Description == "" || division.Url == null)
                    {
                        ViewBag.ResultOfSave = "Заполните все поля.";
                        return View(division);
                    }
                    xml.SaveDivisions(divisions);
                    return Redirect("/Division/Edit/" + id);
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
        public ActionResult AddData(int id)
        {
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                var users = xml.LoadUsers();
                SelectList SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                {
                    Text = u.Login,
                    Value = u.Login,
                    Selected = false
                }));
                ViewBag.Users = SelectListUsers;

                return View();
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddData(int id, Itemprop ip, FormCollection form)
        {
            if (IsAdminUser())
            {
                string userLogin = form["user"].ToString();
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();
                var div = divisions.First(d => d.Id == id);

                var users = xml.LoadUsers();
                ip.ResponsibleUser = users.Any(u => u.Login == userLogin) ? users.First(u => u.Login == userLogin) : null;
                div.AddInfo(ip);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/Edit/" + id);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult AddTable(int id)
        {
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);

                var users = xml.LoadUsers();
                SelectList SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                {
                    Text = u.Login,
                    Value = u.Login,
                    Selected = false
                }));
                ViewBag.Users = SelectListUsers;

                return View();
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddTable(int id, Table tbl, FormCollection form)
        {
            if (IsAdminUser())
            {
                string userLogin = form["user"].ToString();
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();
                var div = divisions.First(d => d.Id == id);
                var users = xml.LoadUsers();
                tbl.ResponsibleUser = users.Any(u => u.Login == userLogin) ? users.First(u => u.Login == userLogin) : null;
                var id_tbl = div.Tables.Count + 1;
                div.AddTable(tbl);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/EditTable?idd=" + id + "&idt=" + id_tbl);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult AddTableData(int div, int tbl, FormCollection form)
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
        [HttpPost]
        public ActionResult AddTableData(int div, int tbl, TableItemprop ip, FormCollection form)
        {
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();
                var table = divisions.First(d => d.Id == div).Tables[tbl - 1];
                if (ip.Value == null || ip.Value == "" ||
                    ip.Description == null || ip.Description == "")
                {
                    ViewBag.Message = "Заполните все обязательные поля.";
                    return View(ip);
                }
                if (form["Type"] == "")
                {
                    ViewBag.Message = "Заполните поле Тип.";
                    return View(ip);
                }
                if (table.TableItemprops.Any(k => k.IsMainTag == true && ip.IsMainTag == true))
                {
                    ViewBag.Message = "Данная таблица уже содержит главный тег.";
                    return View(ip);
                }
                if (table.TableItemprops.Any(k => k.Value == ip.Value && k.IsMainTag == ip.IsMainTag))
                {
                    ViewBag.Message = "Такой атрибут уже существует в таблице.";
                    return View(ip);
                }
                if (ip.IsMainTag == true && (ip.Type == ItempropTypeEnum.Optional || ip.Type == ItempropTypeEnum.Selective))
                {
                    ViewBag.Message = "Главный тег может быть только обязательным.";
                    return View(ip);
                }
                table.AddTableItemprop(ip);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/EditTable?idd=" + div + "&idt=" + tbl);
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
        public ActionResult DeleteData(int id, string name)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && id > 0 && divisions.Any(d => d.Id == id))
            {
                divisions.First(d => d.Id == id).Data.RemoveAll(d => d.Value == name);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/Edit/" + id);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult DeleteTable(int idd, int idt)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && idd > 0 && divisions.Any(d => d.Id == idd))
            {
                var tbl = divisions.First(d => d.Id == idd).Tables[idt - 1];
                divisions.First(d => d.Id == idd).Tables.RemoveAll(t => t == tbl);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/Edit/" + idd);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult DeleteTableData(int idd, int idt, string name)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && divisions.Any(d => d.Id == idd))
            {
                divisions.First(d => d.Id == idd).Tables[idt - 1].TableItemprops.RemoveAll(t => t.Value == name);
                xml.SaveDivisions(divisions);
                return Redirect("/Division/EditTable?idd=" + idd + "&idt=" + idt);
            }
            else
            {
                return Redirect("/Home/Index");
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
                ViewBag.Users = SelectListUsers;
                div.ShortUrl = div.Url.ToString().Remove(0, div.Url.ToString().IndexOf(div.Url.Host) + div.Url.Host.Length + 1);

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
                string userLogin = form["user"] != null ? form["user"].ToString() : null;
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

        [Authorize]
        public ActionResult EditData(int id, string name)
        {
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();

                var users = xml.LoadUsers();
                var div = divisions.First(d => d.Id == id);
                var ip = div.Data.First(d => d.Value == name);
                SelectList SelectListUsers;
                if (ip.ResponsibleUser != null)
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = u.Login == ip.ResponsibleUser.Login ? true : false
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
                ViewBag.id = id;
                return View(ip);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditData(int id, Itemprop ip, FormCollection form)
        {
            if (IsAdminUser())
            {
                string userLogin = form["user"].ToString();
                string oldValue = form["OldValue"].ToString();
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();
                var div = divisions.First(d => d.Id == id);
                var users = xml.LoadUsers();
                var old_ip = div.Data.First(d => d.Value == oldValue);
                old_ip.ResponsibleUser = users.Any(u => u.Login == userLogin) ? users.First(u => u.Login == userLogin) : null;
                old_ip.Value = ip.Value;
                old_ip.Description = ip.Description;
                old_ip.Type = ip.Type;
                xml.SaveDivisions(divisions);
                return Redirect("/Division/Edit/" + id);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        public ActionResult EditTable(int idd, int idt)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            List<Division> divisions = xml.LoadDivisions();
            ViewBag.idd = idd;
            ViewBag.idt = idt;
            if (IsAdminUser() && idd > 0 && divisions.Any(d => d.Id == idd))
            {
                var users = xml.LoadUsers();
                var tbl = divisions.First(d => d.Id == idd).Tables[idt - 1];
                SelectList SelectListUsers;
                if (tbl.ResponsibleUser != null)
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = u.Login == tbl.ResponsibleUser.Login ? true : false
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
                ViewBag.idd = idd;
                ViewBag.idt = idt;
                return View(tbl);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditTable(int idd, int idt, Table table, FormCollection form)
        {
            var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
            var users = xml.LoadUsers();
            ViewBag.idd = idd;
            ViewBag.idt = idt;
            List<Division> divisions = xml.LoadDivisions();
            if (IsAdminUser() && divisions.Any(d => d.Id == idd))
            {
                string userLogin = form["user"] != null ? form["user"].ToString() : null;
                var us = users.Any(u => u.Login == userLogin) ? users.First(u => u.Login == userLogin) : null;
                var tbl = divisions.First(d => d.Id == idd).Tables[idt - 1];
                tbl.Name = table.Name;
                tbl.ResponsibleUser = us;
                tbl.Type = table.Type;
                SelectList SelectListUsers;
                if (tbl.ResponsibleUser != null)
                {
                    SelectListUsers = new SelectList(users.Select(u => new SelectListItem()
                    {
                        Text = u.Login,
                        Value = u.Login,
                        Selected = u.Login == tbl.ResponsibleUser.Login ? true : false
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
                if (tbl.Name == null || tbl.Name == "" || form["Type"] == "")
                {
                    ViewBag.Message = "Заполните все обязательные поля.";
                    return View(tbl);
                }
                xml.SaveDivisions(divisions);
                ViewBag.Message = "Таблица успешно изменена.";
                return View(tbl);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }


        [Authorize]
        public ActionResult EditTableData(int idd, int idt, string name)
        {
            ViewBag.idd = idd;
            ViewBag.idt = idt;
            if (IsAdminUser())
            {
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();

                var users = xml.LoadUsers();
                var div = divisions.First(d => d.Id == idd);
                var ip = div.Tables[idt - 1].TableItemprops.First(d => d.Value == name);
                
                return View(ip);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditTableData(int idd, int idt, TableItemprop ip, FormCollection form)
        {
            ViewBag.idd = idd;
            ViewBag.idt = idt;
            if (IsAdminUser())
            {
                string oldValue = form["OldValue"].ToString();
                var xml = new XmlParser(SettingsManager.Settings.DefaultSiteAddress);
                List<Division> divisions = xml.LoadDivisions();
                var div = divisions.First(d => d.Id == idd);                
                var old_ip = div.Tables[idt - 1].TableItemprops.First(d => d.Value == oldValue);
                old_ip.Value = ip.Value;
                old_ip.Description = ip.Description;
                old_ip.Type = ip.Type;
                old_ip.IsMainTag = ip.IsMainTag;
                xml.SaveDivisions(divisions);
                return Redirect("/Division/EditTable?idd=" + idd + "&idt=" + idt);
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
