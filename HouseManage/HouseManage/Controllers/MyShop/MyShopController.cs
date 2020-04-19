using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Model.Enum;
using House.IService.Shop;
using HouseManage.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Paramter = HouseManage.Models.Request;

namespace HouseManage.Controllers.MyShop
{
    //[AllowAnonymous]
    public class MyShopController : ControllerBase
    {
        private IMyShopSvc _shop;
        public MyShopController(IMyShopSvc shop)
        {
            this._shop = shop;
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

        public JsonResult PayOrders(Paramter.Request request)
        {
            var feeType = request.Conditions["feeType"];
            var Data = _shop.GetPayReminder(OpenID, feeType, request.Page);
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
        #endregion
    }
}