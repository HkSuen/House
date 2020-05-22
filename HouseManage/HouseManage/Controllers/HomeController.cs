using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HouseManage.Models;
using House.IService;
using House.IService.Common;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using House.IService.Common.Sign;
using Microsoft.Extensions.Logging;

namespace HouseManage.Controllers
{
    public class HomeController : ControllerBase
    {
        private IWeiXinSingle _wx = null;
        ILogger<HomeController> _log;
        public HomeController(IWeiXinSingle wx, ILogger<HomeController> log) // wx里面有获取sesion的方法
        {
            this._wx = wx;
            this._log = log;
        }

        /// <summary>
        /// 微信服务器接入验证
        /// </summary>
        [AllowAnonymous]
        public async Task<string> Index()
        {
            return await this._wx.CheckServer();
        }

        [AllowAnonymous]
        public ActionResult Error(string msg)
        {
            ViewBag.Error = msg;
            ViewBag.Title = string.IsNullOrEmpty(msg) ? "权限不足" : "资源未找到";
            return View();
        }


        [AllowAnonymous]
        public string Register()
        {
            return _wx.GetOpenId();
        }

        [HttpPost]
        public JsonResult SDKCON(string url)
        {
            this._log.LogInformation($"请求来源地址：{RequestSource}");
            bool valid = System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.UTF8) == RequestSource;
            object data = _wx.JsApiSignature(RequestSource);
            this._log.LogInformation($"微信配置参数：{JsonConvert.SerializeObject(data)}");
            return OK(data);
        }
    }
}
