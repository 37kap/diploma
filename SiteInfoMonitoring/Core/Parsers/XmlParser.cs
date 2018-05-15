using SiteInfoMonitoring.Core.Enums;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SiteInfoMonitoring.Core.Parsers
{
    public class XmlParser
    {
        private string SiteName;
        private string FilePath;
        private List<User> Users;
        private List<Division> Divisions;
        public Exception Exception;

        public XmlParser(string siteName, string xml_file = "Divisions.XML")
        {
            SiteName = siteName;
            FilePath = xml_file;
        }

        public List<Division> GetDivisions()
        {
            Divisions = new List<Division>();
            try
            {
                if (Users.Count == 0)
                {
                    throw new Exception("Пользователи в XML-файле не найдены");
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(FilePath);
                XmlNode xDivisions = xDoc.GetElementsByTagName("divisions")[0];
                foreach (XmlNode xnode in xDivisions)
                {
                    Division div = new Division();
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("show");
                        if (attr != null)
                        {
                            bool.TryParse(attr.Value, out div.IsShowed);
                        }
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

        public List<User> GetUsers()
        {
            try
            {
                Users = new List<User>();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(FilePath);
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
                        us.Rule = attrRule.Value == "admin" ? RulesEnum.admin : RulesEnum.user;
                    }
                    else
                    {
                        us.Rule = RulesEnum.user;
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
    }
}