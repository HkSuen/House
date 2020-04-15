using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.MyShop
{
    public class MyShopController : Controller
    {
        [AllowAnonymous]
        // GET: MyShop
        public ActionResult Payment()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ShopDetail()
        {
            return View();
        }
    }
}