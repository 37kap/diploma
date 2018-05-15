using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace SiteInfoMonitoring.Core
{
    public class SiteChecker
    {
        private string SiteName;
        public List<Division> Divisions;
        public List<User> Users;
        private string XmlFile;
        public Exception XmlParseException;
        public SiteChecker(string siteName, string xml_file = "Divisions.XML")
        {
            SiteName = siteName.StartsWith("http") ? siteName : "http://" + siteName;
            XmlFile = xml_file;
        }

        public List<Division> GetContentFromXml()
        {
            var xml_parser = new XmlParser(SiteName, XmlFile);
            Users = xml_parser.GetUsers();
            XmlParseException = xml_parser.Exception;
            if (XmlParseException == null)
            {
                Divisions = xml_parser.GetDivisions();
                XmlParseException = xml_parser.Exception;
            }
            return Divisions;
        }

        /// <summary>
        /// Выполняет проверку доступности сайта.
        /// </summary>
        public bool CheckSiteAvailability()
        {
            Uri uri = new Uri(SiteName);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Выполняет проверку наличия обязательных разделов сайта.
        /// </summary>
        public List<Division> CheckDivisionsExist()
        {
            foreach (var d in Divisions)
            {
                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(d.Url);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                }
                catch
                {
                    d.IsExist = false;
                    continue;
                }
                d.IsExist = true;
            }
            return Divisions;
        }
    }
}