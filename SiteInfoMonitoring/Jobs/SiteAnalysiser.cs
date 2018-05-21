using Quartz;
using SiteInfoMonitoring.Core;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiteInfoMonitoring.Jobs
{
    public class SiteAnalysiser : IJob
    {
        public EduSiteParser htmlParser;
        public SiteChecker siteChecker;
        public async Task Execute(IJobExecutionContext context)
        {
            var siteName = SettingsManager.Settings.DefaultSiteAddress;
            if (SettingsManager.Settings.AutoAnalysis && siteName != "")
            {
                siteChecker = new SiteChecker(siteName);
                List<Division> divs = siteChecker.CheckDivisionsExist();
                htmlParser = new EduSiteParser(siteName, divs, siteChecker.XmlParser.GetUsers());
                htmlParser.StartParse(!siteChecker.CheckSiteAvailability(), true);
            }
        }
    }
}