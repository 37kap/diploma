using Calabonga.Portal.Config;

namespace SiteInfoMonitoring.Core.Settings
{
    public class CurrentAppSettings : AppSettings
    {
        public string[] Items { get; set; }

        public int PersonId { get; set; }
    }
}