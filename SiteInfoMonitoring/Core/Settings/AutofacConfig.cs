using Autofac;
using Autofac.Integration.Mvc;
using Calabonga.Portal.Config;
using System.Reflection;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SiteInfoMonitoring.Core.Settings
{
    public static class AutofacConfig
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).AsImplementedInterfaces();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterFilterProvider();

            builder.RegisterType<CacheService>().As<ICacheService>();
            builder.RegisterType<JsonConfigSerializer>().As<IConfigSerializer>();
            builder.RegisterType<AppSettingsManager>().As<IConfigService<CurrentAppSettings>>()
                .WithParameter("configFileName", "AppConfigJson.json");

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}