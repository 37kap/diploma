using SiteInfoMonitoring.Core.Settings;
using SiteInfoMonitoring.Jobs;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SiteInfoMonitoring
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            AutofacConfig.Initialize();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // запуск выполнения работы
            new Thread(t => { Thread.Sleep(60000); SiteAnalysisSheduler.Start(); }).Start();
        }
    }
}
