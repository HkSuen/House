using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using HouseManage.Common;
using HouseManage.Common.Filter;
using HouseManage.Common.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;

namespace HouseManage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<WebEncoderOptions>(options =>
                 options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin,
        UnicodeRanges.CjkUnifiedIdeographs));
            services.AddSession(config =>
            {
                config.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddMvc(config =>
            {  // register authorizefilter .
                config.Filters.Add(typeof(CustomAuthorizeAttribute));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(opt =>
                    {
                        opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();//json字符串大小写原样输出
                    });
            //添加认证Cookie信息
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
             {
                 options.LoginPath = new PathString("/WeChat/home/error");
                 options.AccessDeniedPath = new PathString("/WeChat/home/error");
             });
            return AutofacConfig.Register(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // 添加全局异常捕获，方便记录日志
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "WeChat/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
