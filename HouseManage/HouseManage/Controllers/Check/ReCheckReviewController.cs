﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers.Check
{
    [AllowAnonymous]
    public class ReCheckReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}