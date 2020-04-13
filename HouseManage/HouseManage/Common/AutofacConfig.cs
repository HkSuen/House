using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Data.MSSQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HouseManage.Common
{
    /// <summary>
    /// DDD 设计实现、
    /// 
    /// </summary>
    public class AutofacConfig
    {
        public static IServiceProvider Register(IServiceCollection services)
        {
            //初始化容器
            var builder = new ContainerBuilder();
            //管道寄居
            builder.Populate(services);
            //业务逻辑层所在程序集命名空间
            Assembly service = Assembly.Load("House.IService");
            //接口层所在程序集命名空间
            Assembly repository = Assembly.Load("House.Service");
            //手动注入其他依赖
            RegisterOther(builder);
            //自动注入(容器)
            builder.RegisterAssemblyTypes(service, repository)
                .PublicOnly() // 所有的共有方法
                .Where(t => t.Name.EndsWith("Single")) // 单例生命周期的程序集
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterAssemblyTypes(service, repository)
                .PublicOnly() // 所有的共有方法
                .Where(t => t.Name.EndsWith("Svc")) // 过滤名称为Service的数据集
                .AsImplementedInterfaces(); //是以接口方式进行注入
            //    .InstancePerRequest()       //每次http请求
            //    .PropertiesAutowired();     //属性注入
            //构造
            var ApplicationContainer = builder.Build();
            //将AutoFac反馈到管道中
            return new AutofacServiceProvider(ApplicationContainer);

        }

        private static void RegisterOther(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>().AsImplementedInterfaces();
            //注册数据库
            builder.RegisterType<DataConfig>()
                .As<IDataConfig>().AsImplementedInterfaces();
        }

    }
}
