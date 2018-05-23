using SiteInfoMonitoring.Core.Enums;
using SiteInfoMonitoring.Models;
using SiteInfoMonitoring.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SiteInfoMonitoring.Core.Parsers
{
    public class XmlParser
    {
        private string SiteName;
        private List<User> Users;
        private List<Division> Divisions;
        public Exception Exception;

        public XmlParser(string siteName = "")
        {
            SiteName = siteName;
        }

        #region old methods
        private List<Division> GetDivisions()
        {
            Users = Users == null || Users.Count == 0 ? GetUsers() : Users;
            Divisions = new List<Division>();
            try
            {
                if (Users.Count == 0)
                {
                    throw new Exception("Пользователи в XML-файле не найдены");
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Settings.SettingsManager.Settings.XmlFileDivisions);
                XmlNode xDivisions = xDoc.GetElementsByTagName("divisions")[0];
                foreach (XmlNode xnode in xDivisions)
                {
                    Division div = new Division();
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("show");
                        /*if (attr != null)
                        {
                            bool.TryParse(attr.Value, out div.IsShowed);
                        }
                        attr = null;*/
                        attr = xnode.Attributes.GetNamedItem("url");
                        if (attr != null)
                        {
                            div.Url = new Uri(SiteName + "/" + attr.Value);
                        }
                        attr = null;
                        attr = xnode.Attributes.GetNamedItem("des");
                        if (attr != null)
                        {
                            div.Description = attr.Value;
                        }
                        attr = null;
                        attr = xnode.Attributes.GetNamedItem("user");
                        if (attr != null)
                        {
                            div.ResponsibleUser = Users.First(u => u.Login == attr.Value);
                        }
                    }
                    foreach (XmlNode info in xnode.ChildNodes)
                    {
                        if (info.Name == "data")
                        {
                            var ip = new Itemprop();
                            XmlNode attr = info.Attributes.GetNamedItem("itemprop");
                            ip.Value = attr.Value;
                            ip.Description = info.InnerText;
                            attr = info.Attributes.GetNamedItem("type");
                            if (attr != null)
                            {
                                switch (attr.Value.ToLower())
                                {
                                    case "optional":
                                        ip.Type = ItempropTypeEnum.Optional;
                                        break;
                                    case "required":
                                        ip.Type = ItempropTypeEnum.Required;
                                        break;
                                    case "selective":
                                        ip.Type = ItempropTypeEnum.Selective;
                                        break;
                                }
                            }
                            attr = null;
                            attr = info.Attributes.GetNamedItem("user");
                            if (attr != null)
                            {
                                var us = Users.First(u => u.Login == attr.Value);
                                if (us != div.ResponsibleUser)
                                {
                                    ip.ResponsibleUser = us;
                                }
                            }
                            div.AddInfo(ip);
                        }
                        if (info.Name == "table")
                        {
                            var table = new Table();
                            XmlNode tableAttrType = info.Attributes.GetNamedItem("type");
                            XmlNode tableAttrName = info.Attributes.GetNamedItem("name");
                            if (tableAttrType != null)
                            {
                                switch (tableAttrType.Value.ToLower())
                                {
                                    case "optional":
                                        table.Type = TableTypeEnum.Optional;
                                        break;
                                    case "required":
                                        table.Type = TableTypeEnum.Required;
                                        break;
                                }
                            }
                            if (tableAttrName != null)
                            {
                                table.Name = tableAttrName.Value;
                            }
                            tableAttrName = null;
                            tableAttrName = info.Attributes.GetNamedItem("user");
                            if (tableAttrName != null)
                            {
                                var us = Users.First(u => u.Login == tableAttrName.Value);
                                if (us != div.ResponsibleUser)
                                {
                                    table.ResponsibleUser = us;
                                }
                            }
                            foreach (XmlNode td in info.ChildNodes)
                            {
                                var tip = new TableItemprop();
                                XmlNode attr = td.Attributes.GetNamedItem("itemprop");
                                tip.Value = attr.Value;
                                tip.Description = td.InnerText;
                                attr = td.Attributes.GetNamedItem("type");
                                if (attr != null)
                                {
                                    switch (attr.Value.ToLower())
                                    {
                                        case "optional":
                                            tip.Type = ItempropTypeEnum.Optional;
                                            break;
                                        case "required":
                                            tip.Type = ItempropTypeEnum.Required;
                                            break;
                                        case "selective":
                                            tip.Type = ItempropTypeEnum.Selective;
                                            break;
                                    }
                                }
                                if (td.Name == "tr")
                                {
                                    tip.IsMainTag = true;
                                }
                                table.AddTableItemprop(tip);
                            }
                            div.AddTable(table);

                        }
                    }
                    Divisions.Add(div);
                }
            }
            catch (Exception ex)
            {
                Divisions = new List<Division>();
                Exception = ex;
            }
            return Divisions;
        }

        private List<Division> GetDivisions(string userName)
        {
            Users = GetUserByName(userName);
            Divisions = new List<Division>();
            try
            {
                if (Users.Count == 0)
                {
                    throw new Exception("Пользователь в XML-файле не найден");
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Settings.SettingsManager.Settings.XmlFileDivisions);
                XmlNode xDivisions = xDoc.GetElementsByTagName("divisions")[0];
                foreach (XmlNode xnode in xDivisions)
                {
                    Division div = new Division();
                    bool needDiv = false;
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("show");
                        /*if (attr != null)
                        {
                            bool.TryParse(attr.Value, out div.IsShowed);
                        }*/
                        attr = null;
                        attr = xnode.Attributes.GetNamedItem("url");
                        if (attr != null)
                        {
                            div.Url = new Uri(SiteName + "/" + attr.Value);
                        }
                        attr = null;
                        attr = xnode.Attributes.GetNamedItem("des");
                        if (attr != null)
                        {
                            div.Description = attr.Value;
                        }
                        attr = null;
                        attr = xnode.Attributes.GetNamedItem("user");
                        if (attr != null)
                        {
                            div.ResponsibleUser = Users.Any(u => u.Login == attr.Value) ? Users.First(u => u.Login == attr.Value) : null;
                            if (div.ResponsibleUser != null && div.ResponsibleUser.Login == userName)
                            {
                                needDiv = true;
                            }
                        }
                    }
                    foreach (XmlNode info in xnode.ChildNodes)
                    {
                        if (info.Name == "data")
                        {
                            bool needData = needDiv ? true : false;
                            var ip = new Itemprop();
                            XmlNode attr = info.Attributes.GetNamedItem("itemprop");
                            ip.Value = attr.Value;
                            ip.Description = info.InnerText;
                            attr = info.Attributes.GetNamedItem("type");
                            if (attr != null)
                            {
                                switch (attr.Value.ToLower())
                                {
                                    case "optional":
                                        ip.Type = ItempropTypeEnum.Optional;
                                        break;
                                    case "required":
                                        ip.Type = ItempropTypeEnum.Required;
                                        break;
                                    case "selective":
                                        ip.Type = ItempropTypeEnum.Selective;
                                        break;
                                }
                            }
                            attr = null;
                            attr = info.Attributes.GetNamedItem("user");
                            if (attr != null)
                            {
                                var us = Users.Any(u => u.Login == attr.Value) ? Users.First(u => u.Login == attr.Value) : null;
                                if (us != div.ResponsibleUser)
                                {
                                    ip.ResponsibleUser = us;
                                    if (needDiv)
                                    {
                                        needData = false;
                                    }
                                    else
                                    {
                                        if (us.Login == userName)
                                        {
                                            needData = true;
                                        }
                                    }
                                }
                            }
                            if (needData)
                            {
                                div.AddInfo(ip);
                            }
                        }
                        if (info.Name == "table")
                        {
                            bool needTbl = needDiv ? true : false;
                            var table = new Table();
                            XmlNode tableAttrType = info.Attributes.GetNamedItem("type");
                            XmlNode tableAttrName = info.Attributes.GetNamedItem("name");
                            if (tableAttrType != null)
                            {
                                switch (tableAttrType.Value.ToLower())
                                {
                                    case "optional":
                                        table.Type = TableTypeEnum.Optional;
                                        break;
                                    case "required":
                                        table.Type = TableTypeEnum.Required;
                                        break;
                                }
                            }
                            if (tableAttrName != null)
                            {
                                table.Name = tableAttrName.Value;
                            }
                            tableAttrName = null;
                            tableAttrName = info.Attributes.GetNamedItem("user");
                            if (tableAttrName != null)
                            {
                                var us = Users.Any(u => u.Login == tableAttrName.Value) ? Users.First(u => u.Login == tableAttrName.Value) : null;
                                if (us != div.ResponsibleUser)
                                {
                                    table.ResponsibleUser = us;
                                    if (needDiv)
                                    {
                                        needTbl = false;
                                    }
                                    else
                                    {
                                        if (us.Login == userName)
                                        {
                                            needTbl = true;
                                        }
                                    }
                                }
                            }
                            foreach (XmlNode td in info.ChildNodes)
                            {
                                var tip = new TableItemprop();
                                XmlNode attr = td.Attributes.GetNamedItem("itemprop");
                                tip.Value = attr.Value;
                                tip.Description = td.InnerText;
                                attr = td.Attributes.GetNamedItem("type");
                                if (attr != null)
                                {
                                    switch (attr.Value.ToLower())
                                    {
                                        case "optional":
                                            tip.Type = ItempropTypeEnum.Optional;
                                            break;
                                        case "required":
                                            tip.Type = ItempropTypeEnum.Required;
                                            break;
                                        case "selective":
                                            tip.Type = ItempropTypeEnum.Selective;
                                            break;
                                    }
                                }
                                if (td.Name == "tr")
                                {
                                    tip.IsMainTag = true;
                                }
                                table.AddTableItemprop(tip);
                            }
                            if (needTbl)
                            {
                                div.AddTable(table);
                            }
                        }
                    }
                    if (needDiv || div.Data.Count > 0 || div.Tables.Count > 0)
                    {
                        Divisions.Add(div);
                    }
                }
            }
            catch (Exception ex)
            {
                Divisions = new List<Division>();
                Exception = ex;
            }
            return Divisions;
        }

        private List<User> GetUserByName(string userName)
        {
            try
            {
                Users = new List<User>();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Settings.SettingsManager.Settings.XmlFileUsers);
                XmlNode xDivisions = xDoc.GetElementsByTagName("users")[0];
                foreach (XmlNode xnode in xDivisions)
                {
                    User us = new User();
                    XmlNode attrLogin = xnode.Attributes.GetNamedItem("login");
                    if (attrLogin != null)
                    {
                        us.Login = attrLogin.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    if (us.Login != userName)
                    {
                        continue;
                    }

                    XmlNode attrName = xnode.Attributes.GetNamedItem("name");
                    if (attrName != null)
                    {
                        us.Name = attrName.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrEmail = xnode.Attributes.GetNamedItem("email");
                    if (attrEmail != null)
                    {
                        us.Email = attrEmail.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrPass = xnode.Attributes.GetNamedItem("password");
                    if (attrPass != null)
                    {
                        us.Password = attrPass.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrRule = xnode.Attributes.GetNamedItem("rule");
                    if (attrRule != null)
                    {
                        us.Role = attrRule.Value == "admin" ? RolesEnum.admin : RolesEnum.user;
                    }
                    else
                    {
                        us.Role = RolesEnum.user;
                    }
                    Users.Add(us);
                    break;
                }
            }
            catch (Exception ex)
            {
                Users = new List<User>();
                Exception = ex;
            }
            return Users;
        }

        private List<User> GetUsers()
        {
            try
            {
                Users = new List<User>();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Settings.SettingsManager.Settings.XmlFileUsers);
                XmlNode xDivisions = xDoc.GetElementsByTagName("users")[0];
                foreach (XmlNode xnode in xDivisions)
                {
                    User us = new User();
                    XmlNode attrName = xnode.Attributes.GetNamedItem("name");
                    if (attrName != null)
                    {
                        us.Name = attrName.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrEmail = xnode.Attributes.GetNamedItem("email");
                    if (attrEmail != null)
                    {
                        us.Email = attrEmail.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrLogin = xnode.Attributes.GetNamedItem("login");
                    if (attrLogin != null)
                    {
                        us.Login = attrLogin.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrPass = xnode.Attributes.GetNamedItem("password");
                    if (attrPass != null)
                    {
                        us.Password = attrPass.Value;
                    }
                    else
                    {
                        throw new Exception("Указаны не все атрибуты пользователя");
                    }

                    XmlNode attrRule = xnode.Attributes.GetNamedItem("rule");
                    if (attrRule != null)
                    {
                        us.Role = attrRule.Value == "admin" ? RolesEnum.admin : RolesEnum.user;
                    }
                    else
                    {
                        us.Role = RolesEnum.user;
                    }
                    Users.Add(us);
                }
            }
            catch (Exception ex)
            {
                Users = new List<User>();
                Exception = ex;
            }
            return Users;
        }
        #endregion

        public void SaveDivisions(List<Division> divisions)
        {
            var divs = divisions.Select(d => new Serialization.DivisionSerializable()
            {
                Url = d.Url.ToString().Remove(0, d.Url.ToString().IndexOf(SiteName) + SiteName.Length + 1),
                Description = d.Description,
                Data = d.Data != null ? d.Data.Select(dt => new Serialization.ItempropSerializable()
                {
                    Description = dt.Description,
                    Value = dt.Value,
                    Type = dt.Type,
                    User = dt.ResponsibleUser != null ? dt.ResponsibleUser.Login : null
                }).ToList() : null,
                User = d.ResponsibleUser != null ? d.ResponsibleUser.Login : null,
                Tables = d.Tables != null ? d.Tables.Select(t => new TableSerializable()
                {
                    Type = t.Type,
                    Name = t.Name,
                    User = t.ResponsibleUser != null ? t.ResponsibleUser.Login : null,
                    TableItemprops = t.TableItemprops != null ? t.TableItemprops.Select(tip => new TableItempropSerializable()
                    {
                        Description = tip.Description,
                        Type = tip.Type,
                        User = tip.ResponsibleUser != null ? tip.ResponsibleUser.Login : null,
                        Value = tip.Value,
                        IsMainTag = tip.IsMainTag
                    }).ToList() : null
                }).ToList() : null,
            }).ToArray();
            XmlSerializer formatter = new XmlSerializer(typeof(DivisionSerializable[]));

            File.Delete(Settings.SettingsManager.Settings.XmlFileDivisions);
            try
            {
                using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileDivisions, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, divs);
                }
            }
            catch (Exception exception)
            {
                using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileDivisions.ToLower().Replace(".xml", "_backup.xml"), FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, divs);
                }
            }
        }

        public List<Division> LoadDivisions()
        {
            List<User> users = LoadUsers();
            List<Division> divs = new List<Division>();
            DivisionSerializable[] divisions = null;
            XmlSerializer formatter = new XmlSerializer(typeof(DivisionSerializable[]));
            using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileDivisions, FileMode.OpenOrCreate))
            {
                divisions = (DivisionSerializable[])formatter.Deserialize(fs);
            }
            SiteName = SiteName == "" ? Settings.SettingsManager.Settings.DefaultSiteAddress : SiteName;
            divs = divisions.Select(d => new Division()
            {
                Url = new Uri(SiteName + "/" + d.Url.ToString()),
                Description = d.Description,
                Data = d.Data != null ? d.Data.Select(dt => new Itemprop()
                {
                    Description = dt.Description,
                    Value = dt.Value,
                    Type = dt.Type,
                    ResponsibleUser = dt.User != null && dt.User != "" && users.Any(u => u.Login == dt.User) ?
                                      users.First(u => u.Login == dt.User) : null
                }).ToList() : null,
                ResponsibleUser = d.User != null && d.User != "" && users.Any(u => u.Login == d.User) ?
                                  users.First(u => u.Login == d.User) : null,
                Tables = d.Tables != null ? d.Tables.Select(t => new Table()
                {
                    Type = t.Type,
                    Name = t.Name,
                    ResponsibleUser = t.User != null && t.User != "" && users.Any(u => u.Login == t.User) ?
                                  users.First(u => u.Login == t.User) : null,
                    TableItemprops = t.TableItemprops != null ? t.TableItemprops.Select(tip => new TableItemprop()
                    {
                        Description = tip.Description,
                        Type = tip.Type,
                        ResponsibleUser = tip.User != null && tip.User != "" && users.Any(u => u.Login == tip.User) ?
                                  users.First(u => u.Login == tip.User) : null,
                        Value = tip.Value,
                        IsMainTag = tip.IsMainTag
                    }).ToList() : null
                }).ToList() : null,
            }).ToList();
            int k = 1;
            foreach (var div in divs)
            {
                div.Id = k++;
            }
            return divs;
        }

        public List<Division> LoadDivisions(string userName)
        {
            User user = LoadUserByName(userName);
            List<Division> divs = new List<Division>();
            DivisionSerializable[] divisions = null;
            XmlSerializer formatter = new XmlSerializer(typeof(DivisionSerializable[]));
            using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileDivisions, FileMode.OpenOrCreate))
            {
                divisions = (DivisionSerializable[])formatter.Deserialize(fs);
            }
            SiteName = SiteName == "" ? Settings.SettingsManager.Settings.DefaultSiteAddress : SiteName;
            divs = divisions.Where(d => d.User == userName || d.Data.Any(dt => dt.User == userName) || d.Tables.Any(t => t.User == userName))
                .Select(d => new Division()
                {
                    Url = new Uri(SiteName + "/" + d.Url.ToString()),
                    Description = d.Description,
                    Data = d.Data != null ? d.Data.Select(dt => new Itemprop()
                    {
                        Description = dt.Description,
                        Value = dt.Value,
                        Type = dt.Type,
                        ResponsibleUser = dt.User != null && dt.User != "" && userName == dt.User ? user : null
                    }).ToList() : null,
                    ResponsibleUser = d.User != null && d.User != "" && userName == d.User ? user : null,
                    Tables = d.Tables != null ? d.Tables.Select(t => new Table()
                    {
                        Type = t.Type,
                        Name = t.Name,
                        ResponsibleUser = t.User != null && t.User != "" && userName == t.User ? user : null,
                        TableItemprops = t.TableItemprops != null ? t.TableItemprops.Select(tip => new TableItemprop()
                        {
                            Description = tip.Description,
                            Type = tip.Type,
                            ResponsibleUser = tip.User != null && tip.User != "" && userName == tip.User ? user : null,
                            Value = tip.Value,
                            IsMainTag = tip.IsMainTag
                        }).ToList() : null
                    }).ToList() : null,
                }).ToList();
            divs.Select(d => d.Data.RemoveAll(data => (d.ResponsibleUser == null || d.ResponsibleUser.Login != userName) && (data.ResponsibleUser == null || data.ResponsibleUser.Login != userName)));
            divs.Select(d => d.Tables.RemoveAll(t => (d.ResponsibleUser == null || d.ResponsibleUser.Login != userName) && (t.ResponsibleUser == null || t.ResponsibleUser.Login != userName)));
            return divs;
        }

        public void SaveUsers(List<User> users)
        {
            if (users.Any(u => u.Email == null || u.Email == "" || u.Login == null || u.Login == "" ||
            u.Password == null || u.Password == "" || u.Name == null || u.Name == ""))
            {
                throw new Exception("Должны быть заполнены все поля.");
            }
            var us = users.Select(d => new UserSerializable()
            {
                Email = d.Email,
                Login = d.Login,
                Name = d.Name,
                Password = d.Password,
                Role = d.Role
            }).ToArray();
            XmlSerializer formatter = new XmlSerializer(typeof(UserSerializable[]));
            File.Delete(Settings.SettingsManager.Settings.XmlFileUsers);
            try
            {
                using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileUsers, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, us);
                }
            }
            catch (Exception exception)
            {
                using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileUsers.ToLower().Replace(".xml", "_backup.xml"), FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, us);
                }
            }
        }

        public List<User> LoadUsers()
        {
            try
            {
                List<UserSerializable> users = null;
                XmlSerializer formatter = new XmlSerializer(typeof(List<UserSerializable>));
                using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileUsers, FileMode.OpenOrCreate))
                {
                    users = (List<UserSerializable>)formatter.Deserialize(fs);
                }
                return users.Select(d => new User()
                {
                    Email = d.Email,
                    Login = d.Login,
                    Name = d.Name,
                    Password = d.Password,
                    Role = d.Role
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<User> { new User() { Email = "asasas@asas.ru", Login = "superman", Name = "Superman", Password = "2w3e4r", Role = RolesEnum.admin } };
            }
        }

        public User LoadUserByName(string userName)
        {
            List<UserSerializable> users = null;
            XmlSerializer formatter = new XmlSerializer(typeof(List<UserSerializable>));
            using (FileStream fs = new FileStream(Settings.SettingsManager.Settings.XmlFileUsers, FileMode.OpenOrCreate))
            {
                users = (List<UserSerializable>)formatter.Deserialize(fs);
            }
            return users.Where(u => u.Login == userName).Select(d => new User()
            {
                Email = d.Email,
                Login = d.Login,
                Name = d.Name,
                Password = d.Password,
                Role = d.Role
            }).First();
        }
    }
}