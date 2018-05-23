using SiteInfoMonitoring.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteInfoMonitoring.Models
{
    public class Division
    {

        public int Id { get; set; }

        [Display(Name = "Ссылка на страницу (без указания хоста)")]
        [Required(ErrorMessage = "Заполните поле. Например, \"sveden/common\"")]
        public Uri Url { get; set; }


        [Display(Name = "Ссылка на страницу (без указания хоста)")]
        [Required(ErrorMessage = "Заполните поле. Например, \"sveden/common\"")]
        public string ShortUrl { get; set; }

        public List<Uri> Refs;

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Заполните поле")]
        public string Description { get; set; }
        public bool IsExist = false;

        [Display(Name = "Ответственный пользователь")]
        [Required(ErrorMessage = "Заполните поле")]
        public User ResponsibleUser { get; set; }
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