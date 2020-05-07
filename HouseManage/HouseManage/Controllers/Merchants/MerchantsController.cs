using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.MSSQL.Model.Data;
using House.IService.Merchants;
using House.IService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.Merchants
{
    [Authorize(Roles = "Admin,Inspector")]
    //[AllowAnonymous]
    public class MerchantsController : ControllerBase
    {
        private IMerchantSvc _machantSvc = null;
        public MerchantsController(IMerchantSvc machantSvc)
        {
            this._machantSvc = machantSvc;
        }
        #region 商铺信息

      
        /// <summary>
        /// 商铺信息主页
        /// </summary>
        /// <returns></returns>
        public IActionResult ShopInfos()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult GetBaseData(DictionaryModel model)
        {
            Dictionary<string, object> dic = _machantSvc.GetBaseData(model);
            return Json(dic);
        }
        
        [HttpGet]
        public IActionResult GetMerchantList(wy_houseinfo model)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantList(model,UserID, URoleE.Value);
            return Json(dic);
        }
        
        [HttpGet]
        public IActionResult GetMerchantListByPage(string FWBH,int FWSX,string SSQY,string LSFGS,int page,int size )
        {

            Dictionary<string, object> dic = _machantSvc.GetMerchantListByPage( FWBH,  FWSX,  SSQY,  LSFGS,  page,  size, UserID, URoleE.Value);
            return Json(dic);
        }
        [HttpGet]
        public IActionResult GetMerchantListDetail(string FWID)
        {
            ViewBag.FWID = FWID;
            return View();
        }
        [HttpGet]
        public IActionResult GetMerchantListHouseDetail(string FWID)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantListHouseDetail(FWID);
            return  Json(dic);
        }
        [HttpGet]
        public IActionResult GetMerchantListShopDetail(string FWID)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantListShopDetail(FWID);
            return Json(dic);
        }
        #endregion
        #region 商户信息

        
        /// <summary>
        /// 商户信息主页
        /// </summary>
        /// <returns></returns>
        public IActionResult MerchantInfo() {
            return View();
        }
        [HttpGet]
        public IActionResult GetShopInfoListByPage(string ShopName, int FWSX, string SSQY, string LSFGS, int page, int size)
        {

            Dictionary<string, object> dic = _machantSvc.GetShopInfoListByPage(ShopName, FWSX, SSQY, LSFGS, page, size, UserID, URoleE.Value);
            return Json(dic);
        }
        [HttpGet]
        public IActionResult GetShopInfoList(string ShopName, int FWSX, string SSQY, string LSFGS)
        {

            Dictionary<string, object> dic = _machantSvc.GetShopInfoList(ShopName, FWSX, SSQY, LSFGS, UserID, URoleE.Value);
            return Json(dic);
        }
        [HttpGet]
        public IActionResult GetMerchantShopInfoListDetail(string FWID, string CZ_SHID)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantShopInfoListDetail(FWID,CZ_SHID);
            return Json(dic);
        }
        [HttpGet]
        public IActionResult MerchantShopInfoListDetail(string FWID, string CZ_SHID) {
            ViewBag.FWID = FWID;
            ViewBag.CZ_SHID = CZ_SHID;
            return View();
        }
        [HttpGet]
        public IActionResult GetMerchantShopJiaoFeiList(string FWID, string CZ_SHID)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantShopJiaoFeiList(FWID, CZ_SHID);
            return Json(dic);
        }
        
        #endregion
    }
}