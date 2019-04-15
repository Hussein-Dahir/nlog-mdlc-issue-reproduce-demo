using ClientPortalAPI.NLogLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;
using NLog.Time;
using System;
using System.IO;

namespace TodoApi.NLogLogger
{
    public class NLogInitializer
    {
        public static void ConfigureNLog(IServiceProvider serviceProvider)
        {
            var config = new LoggingConfiguration();

            string targetName = "MyCustomTarget";
            Target.Register<MyCustomTarget>(targetName);

            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            MyCustomTarget myCustomTarget = new MyCustomTarget(
                httpContextAccessor)
            {
                Name = targetName
            };

            config.AddTarget(myCustomTarget);

            config.AddRuleForAllLevels(myCustomTarget);

            TimeSource.Current = new FastUtcTimeSource();

            NLog.LogManager.Configuration = config;
        }
    }
}
