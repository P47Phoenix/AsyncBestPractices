using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WebApplication
{
    public class Global : HttpApplication
    {
        private static Lazy<ThreadPoolLogger> _ThreadPoolLogger = new Lazy<ThreadPoolLogger>(() => new ThreadPoolLogger());

        public static ThreadPoolLogger ThreadPoolLogger => _ThreadPoolLogger.Value;

        private static LoggingLevelSwitch _loggingLevelSwitch = new LoggingLevelSwitch();

        public LoggingLevelSwitch LogEventLevel => _loggingLevelSwitch;

        void Application_Start(object sender, EventArgs e)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(LogEventLevel)
                .WriteTo.ColoredConsole()
                .CreateLogger();

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            _ThreadPoolLogger.Value.Start();
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}