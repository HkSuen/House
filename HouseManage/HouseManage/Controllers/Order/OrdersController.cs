using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.Order
{
    public class OrdersController : Controller
    {
        public JsonResult CreateOrder()
        {
            return Json(new { });
        }

    }
}