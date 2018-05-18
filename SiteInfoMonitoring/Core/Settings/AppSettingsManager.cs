using Calabonga.Portal.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteInfoMonitoring.Core.Settings
{
    public class AppSettingsManager : ConfigServiceBase<CurrentAppSettings>
    {
        public AppSettingsManager(IConfigSerializer serializer, ICacheService cacheService) : base(serializer, cacheService)
        {
        }

        public AppSettingsManager(string configFileName, IConfigSerializer serializer, ICacheService cacheService) : base(configFileName, serializer, cacheService)
        {
        }
    }
}