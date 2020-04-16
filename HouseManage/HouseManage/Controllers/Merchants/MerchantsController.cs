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
    [AllowAnonymous]
    public class MerchantsController : Controller
    {
        private IMerchantSvc _machantSvc = null;
        public MerchantsController(IMerchantSvc machantSvc)
        {
            this._machantSvc = machantSvc;
        }
        
        public IActionResult ShopInfos()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetBaseData(DictionaryModel model)
        {
            Dictionary<string, object> dic = _machantSvc.GetBaseData(model);
            return Json(dic);
        }
        
        [HttpGet]
        public IActionResult GetMerchantList(wy_houseinfo model)
        {
            Dictionary<string, object> dic = _machantSvc.GetMerchantList(model);
            return Json(dic);
        }
        
        [HttpGet]
        public IActionResult GetMerchantListByPage(string FWBH,int FWSX,string SSQY,string LSFGS,int page,int size )
        {

            Dictionary<string, object> dic = _machantSvc.GetMerchantListByPage( FWBH,  FWSX,  SSQY,  LSFGS,  page,  size);
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
    }
}