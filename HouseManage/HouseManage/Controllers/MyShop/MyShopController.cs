﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Common;
using House.IService.Model.Enum;
using House.IService.Shop;
using HouseManage.Models.Enum;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Paramter = HouseManage.Models.Request;

namespace HouseManage.Controllers.MyShop
{
    //[Authorize(Roles = "Admin,Inspector,Merchant")]
    public class MyShopController : ControllerBase
    {
        ILogger<MyShopController> _log;
        private IMyShopSvc _shop;
        public MyShopController(IMyShopSvc shop,ILogger<MyShopController> _loger)
        {
            this._shop = shop;
            this._log = _loger;
        }


        #region 缴费管理

        // GET: MyShop
        public ActionResult Payment()
        {
            return View();
        }

        public JsonResult PayReminder(Paramter.Request request)
        {
            var feeType = request.Conditions["feeType"];
            var Data = _shop.GetPayReminder(OpenID, feeType, request.Page);
            return Json(new Paramter.Response() {
                code = ResultCode.SCCUESS,
                data = new { request.Page, Data }
            });
        }

        /// <summary>
        /// 获取最新的一条未交费的提醒单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNewPayReminder()
        {
            var Data = _shop.GetPayReminder(OpenID);
            return Json(new Paramter.Response()
            {
                code = ResultCode.SCCUESS,
                data = new { Data }
            });
        }


        public JsonResult PayOrders(Paramter.Request request)
        {
            var feeType = request.Conditions["feeType"];
            //获取已支付的订单
            var Data = _shop.GetPayRecord(OpenID, feeType,1, request.Page); 
            return Json(new Paramter.Response()
            {
                code = ResultCode.SCCUESS,
                data = new { request.Page, Data }
            });
        }


        public JsonResult TypesFee()
        {
            return Json(Fee.Types);
        }
        public JsonResult TypesFeeAndMech()
        {
            return Json(CommonFiled.TypesAndMechName);
        }

        #endregion

        #region 商铺信息

        public ActionResult ShopDetail()
        {
            return View();
        }

        /// <summary>
        /// 获取商铺信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetShopDetails()
        {
            var ListInfo = _shop.GetShopDetails(OpenID);
            return Json(ListInfo);
        }

        public JsonResult GetWater(string WaterId)
        {
            if (string.IsNullOrEmpty(WaterId))
            {
                return Data(ResultCode.PARAMS_IS_NULL);
            }
            var WaterInfo = _shop.GetWater(WaterId);
            return OK(WaterInfo);
        }

        public JsonResult GetElectricity(string CollectId ,string ElectricityId)
        {
            if (string.IsNullOrEmpty(CollectId) || string.IsNullOrEmpty(ElectricityId))
            {
                return Data(ResultCode.PARAMS_IS_NULL);
            }
            var ElectricityInfo = _shop.GetElectricity(CollectId, ElectricityId);
            return OK(ElectricityInfo);
        }
        #endregion
    }
}