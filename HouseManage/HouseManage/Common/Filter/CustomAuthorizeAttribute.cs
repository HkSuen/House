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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HouseManage.Common.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly IWeiXinSingle _wx;
        private readonly IUsersSvc _users;
        //private readonly IUser

        public CustomAuthorizeAttribute(IWeiXinSingle wx,IUsersSvc users)
        {
            this._wx = wx;
            this._users = users;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if (HasAllowAnonymous(context))
            {
                return;
            }
            else
            {
                //1.访问界面的时候，校验是否登录过，这里后续补充Cookie验证或者其他验证。
                //授权验证
                if (!context.HttpContext.User.Identity.IsAuthenticated) //未授权
                {
                    var path = context.HttpContext.Request.Path;
                    var querString = context.HttpContext.Request.QueryString;
                    //2.如果未授权尝试获取openid。
                    string RedirectUrls = string.Empty;
                    var openId = _wx.GetOpenId(out RedirectUrls);
                    //var openId = "oAY4Pv4h8eyBSAs4O8psFw6omlsg";
                    if (!string.IsNullOrEmpty(openId)) //opneId参数为空，
                    {
                        //3.检查openid是否被注册过，如果没有被注册过跳转Register注册界面。
                        UserDto userDto = this._users.FindUserByOpenId(openId);
                        if (userDto == null || string.IsNullOrEmpty(userDto.PHONE)) //未注册
                        {
                            string registerUrl = CommonFiled.RegisterUrl + "?uid=" + openId + "&redirect=" + path + querString;
                            context.Result = new RedirectResult(registerUrl); //增加回调地址
                            return;
                        }
                        //4.openid 注册过以后，增加登录授权。
                        var ClaimsUser = new ClaimsPrincipal(new ClaimsIdentity(new[] 
                        {
                            new Claim("Uid",userDto.ID),
                            new Claim(ClaimTypes.Name, openId),
                            new Claim(ClaimTypes.Role,userDto.AUTHORITY)
                        },CookieAuthenticationDefaults.AuthenticationScheme));
                        await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsUser);
                        context.HttpContext.User = ClaimsUser;
                    }
                    else
                    {
                        //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden); //OPENID为空直接拒绝访问
                        if (!string.IsNullOrEmpty(RedirectUrls))
                        {
                            context.Result = new RedirectResult(RedirectUrls);
                        }
                        return;
                    }
                }
                //var user = context.HttpContext.User;
                //var isAuthorized = user.Identity.IsAuthenticated;
                //if (!isAuthorized)
                //{
                //    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                //    return;
                //}
            }
        }

        /// <summary>
        /// 判断当前请求的context.Filters是否包含IAllowAnonymousFilter；
        /// 包含代表当前Action需要忽略权限验证逻辑。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool HasAllowAnonymous(AuthorizationFilterContext context)
            => context.Filters.Any(item => item is IAllowAnonymousFilter);
    }
}
