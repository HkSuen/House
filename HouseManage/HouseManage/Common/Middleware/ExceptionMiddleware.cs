using Autofac.Core;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseManage.Common.Middleware
{
    /// <summary>
    /// Verify permissions
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        ILogger<ExceptionMiddleware> _loger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> _log)
        {
            _next = next;
            _loger = _log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this._next.Invoke(context);
            }
            catch (Exception ex)
            {
                _loger.LogError(ex,"Server<br /> Error!!");
                string html = $"<script>window.location.href='/WeChat/home/error?msg={ex.Message}'</script>";
                await context.Response.WriteAsync(html);
            }
        }

        private string ObjStr(object obj) {
            if (null == obj)
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj);
        }


        private string LogHttpRequest(HttpRequest request)
        {
            string logInfo = "";
            return logInfo;
        }

    }
}
