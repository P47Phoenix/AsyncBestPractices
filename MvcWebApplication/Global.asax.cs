using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;
using Serilog.Core;
using WebApplication;

namespace MvcWebApplication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static Lazy<ThreadPoolLogger> _ThreadPoolLogger = new Lazy<ThreadPoolLogger>(() => new ThreadPoolLogger());

        public static ThreadPoolLogger ThreadPoolLogger => _ThreadPoolLogger.Value;

        private static LoggingLevelSwitch _loggingLevelSwitch = new LoggingLevelSwitch();

        public LoggingLevelSwitch LogEventLevel => _loggingLevelSwitch;
        protected void Application_Start()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(LogEventLevel)
                .WriteTo.ColoredConsole()
                .CreateLogger();

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            ThreadPoolLogger.Start();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
