using SiteInfoMonitoring.Core;
using System;
using System.Collections.Generic;

namespace SiteInfoMonitoring.Models
{
    public class Division
    {
        public bool IsShowed = true;
        public Uri Url;
        public List<Uri> Refs;
        public string Description;
        public bool IsExist = false;
        public User ResponsibleUser;
        public List<Itemprop> Data;
        public List<Table> Tables;

        public Division()
        {
            Data = new List<Itemprop>();
            Tables = new List<Table>();
            Refs = new List<Uri>();
        }

        public void AddInfo(Itemprop ip)
        {
            Data.Add(ip);
        }
        public void AddTable(Table table)
        {
            Tables.Add(table);
        }
        public void AddRef(Uri uri)
        {
            Refs.Add(uri);
        }
        public void AddRef(string url)
        {
            if (!url.StartsWith(Url.Scheme))
            {
                if (url.StartsWith(Url.Host))
                {
                    url = Url.Scheme + "://" + url;
                }
                else
                {
                    url = url[0] == '/' ? url : '/' + url;
                    url = Url.Scheme + "://" + Url.Host + url;
                }
            }
            Uri uri = new Uri(url);
            Refs.Add(uri);
        }
    }
}