using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Models
{
    public class UserProblems
    {
        public Division Division { get; set; }
        public List<string> Items { get; set; }

        public UserProblems()
        {
            Items = new List<string>();
        }
    }
}