using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Merchants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.Merchants
{
    public class MerchantsController : Controller
    {
        private IMerchantSvc _machantSvc = null;
        public MerchantsController(IMerchantSvc machantSvc)
        {
            this._machantSvc = machantSvc;
        }
        [AllowAnonymous]
        public IActionResult ShopInfos()
        {
            return View();
        }
    }
}