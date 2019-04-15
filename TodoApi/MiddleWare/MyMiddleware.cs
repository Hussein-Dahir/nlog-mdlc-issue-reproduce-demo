using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TodoApi.MiddleWare
{
    /// <summary>
    /// Middleware to handle all requests and Exceptions
    /// </summary>
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public MyMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger("MyCustomLogger");
        }
        /// <summary>
        /// Process the Request and Catch Exceptions
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var eventID = Guid.NewGuid().ToString() + "-" + DateTime.UtcNow.Ticks.ToString();

            try
            {
                await _next(context);

                if (!NLog.MappedDiagnosticsLogicalContext.Contains("recordId"))
                {
                    NLog.MappedDiagnosticsLogicalContext.Set("recordId", 200);
                }

                _logger.LogInformation(new EventId(0, eventID), "log msg");
            }
            catch (Exception e)
            {
                NLog.MappedDiagnosticsLogicalContext.Set("customMsg", "Exception logged in MiddleWare");
                _logger.LogCritical(new EventId(0, eventID), e, e.Message);
            }
            finally
            {
                //clear NLog MappedDiagnosticsLogicalContext from any stored data
                //NLog.MappedDiagnosticsLogicalContext.Clear();
            }
        }
    }
}
