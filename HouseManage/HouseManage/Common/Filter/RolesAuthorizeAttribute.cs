using House.IService.Common;
using House.IService.Model.Dto;
using House.IService.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HouseManage.Common.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RolesAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        //此变量不是父类的
        public new string[] Roles { get;set; }
        //private readonly IUser

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!Roles.Any(context.HttpContext.User.IsInRole)) {
                context.Result = new RedirectResult("/WeChat/Home/Error?1");
                return;
            }
        }
    }
}
