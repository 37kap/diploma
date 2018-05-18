using SiteInfoMonitoring.Core.Enums;
using System.Collections.Generic;

namespace SiteInfoMonitoring.Models
{
    public class User
    {
        public string Name;
        public string Email;
        public string Login;
        public string Password;
        public RolesEnum Role;
        public List<string> Problems = new List<string>();
    }
}