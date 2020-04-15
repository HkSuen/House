using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.MyShop
{
    //[AllowAnonymous]
    public class MyShopController : Controller
    {
        // GET: MyShop
        public ActionResult Payment()
        {
            return View();
        }


        public ActionResult ShopDetail()
        {
            return View();
        }
    }
}