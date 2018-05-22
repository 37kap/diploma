using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SiteInfoMonitoring.Core
{
    public class SiteChecker
    {
        private string SiteName;
        private List<Division> Divisions;
        public XmlParser XmlParser;
        public Exception XmlException;
        public SiteChecker(string siteName)
        {
            try
            {
                SiteName = siteName.StartsWith("http") ? siteName : "http://" + siteName;
                XmlParser = new XmlParser(SiteName);
                Divisions = XmlParser.LoadDivisions();
            }
            catch (Exception ex)
            {
                XmlException = ex;
            }
        }

        public SiteChecker(string siteName, string userName)
        {
            try
            {
                SiteName = siteName.StartsWith("http") ? siteName : "http://" + siteName;
                XmlParser = new XmlParser(SiteName);
                Divisions = XmlParser.LoadDivisions(userName);
            }
            catch (Exception ex)
            {
                XmlException = ex;
            }
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