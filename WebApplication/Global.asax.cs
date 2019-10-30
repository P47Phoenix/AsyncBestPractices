using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplication
{
    public class Global : HttpApplication
    {
        private static Lazy<ThreadPoolLogger> _ThreadPoolLogger = new Lazy<ThreadPoolLogger>(() => new ThreadPoolLogger());

        public static ThreadPoolLogger ThreadPoolLogger => _ThreadPoolLogger.Value;

        void Application_Start(object sender, EventArgs e)
        {
            _ThreadPoolLogger.Value.Start();
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}