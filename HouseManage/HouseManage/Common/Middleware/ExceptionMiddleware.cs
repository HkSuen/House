using Microsoft.AspNetCore.Http;
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

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await this._next.Invoke(context);
            }
            catch (Exception ex)
            {
                string html = $"<script>window.location.href='../home/error?msg={ex.Message}'</script>";
                await context.Response.WriteAsync(html);
            }
        }
    }
}
