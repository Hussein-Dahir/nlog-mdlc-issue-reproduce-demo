using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientPortalAPI.NLogLogger
{
    [Target("MyCustomTarget")]
    public class MyCustomTarget : AsyncTaskTarget
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonLayout dbJsonLayout = new JsonLayout()
        {
            Attributes =
                {
                    new JsonAttribute("action", "${aspnet-MVC-Action}"),
                    new JsonAttribute("requestUrl", "${aspnet-Request-Url:IncludeQueryString=true}"),
                    new JsonAttribute("customMsg", "${mdlc:item=customMsg}"),
                    new JsonAttribute("recordId", "${mdlc:item=recordId}"),
                    new JsonAttribute("message", "${message}"),
                    new JsonAttribute("exception", "${exception:format=toString,Data:maxInnerExceptionLevel=10}"),
                }
        };

        public MyCustomTarget(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Layout = dbJsonLayout;
        }

        protected override async Task WriteAsyncTask(LogEventInfo logEventInfo, CancellationToken cancellationToken)
        {
            string jsonData = Layout.Render(logEventInfo);

            JObject resource = JObject.Parse(jsonData);

            //you may use debug to see object contents
            DecodedJsonData dataObject = resource.ToObject<DecodedJsonData>();

            // use db context with [ASYNC] call to write log object to DB

            // I am just awaiting a completed task here to make the method async
            Task completedTask = Task.CompletedTask;
            await completedTask;
        }

        private class DecodedJsonData
        {
            public string Action { get; set; }
            public string RequestUrl { get; set; }
            public string CustomMsg { get; set; }
            public long RecordId { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
        }
    }
}
