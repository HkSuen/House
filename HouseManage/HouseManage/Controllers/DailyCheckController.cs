using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers
{
    [AllowAnonymous]
    public class DailyCheckController : Controller
    {
        private IDailyCheckSvc dailyCheckSvc;
        public DailyCheckController(IDailyCheckSvc dailyCheckSvc)
        {
            this.dailyCheckSvc = dailyCheckSvc;
        }
        public IActionResult Index()
        {
            return View(dailyCheckSvc.GetTaskInfo("", "",""));
        }
        [HttpGet]
        public IActionResult GetTaskInfo(string status, string starttime,string endtime)
        {
            return View("../DailyCheck/Index", dailyCheckSvc.GetTaskInfo(status, starttime, endtime));
            //return Ok(dailyCheckSvc.GetTaskInfo(status, starttime, endtime));
        }

        public IActionResult DailyCheckDetail(string RWBH)
        {
            return View();
            //return Ok(dailyCheckSvc.GetTaskInfo(status, starttime, endtime));
        }
    }
}