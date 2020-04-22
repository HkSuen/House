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

namespace HouseManage.Controllers
{
    public class HomeController : Controller
    {
        private IWeiXinSingle _wx = null;
        public HomeController(IWeiXinSingle wx) // wx里面有获取sesion的方法
        {
            this._wx = wx;
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
            return View();
        }


        [AllowAnonymous]
        public string Register()
        {
            return _wx.GetOpenId();
        }


    }
}
