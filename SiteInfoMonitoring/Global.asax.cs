﻿using SiteInfoMonitoring.Core.Settings;
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
        }
    }
}
