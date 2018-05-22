using SiteInfoMonitoring.Core.Enums;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SiteInfoMonitoring.Serialization
{
    [Serializable]
    public class UserSerializable
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public RolesEnum Role { get; set; }

        public UserSerializable()
        {
        }
    }
}