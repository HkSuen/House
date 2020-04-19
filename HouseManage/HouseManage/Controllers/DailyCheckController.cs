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
            string OPEN_ID = Request.HttpContext.User.Identity.Name;
            return View(dailyCheckSvc.GetTaskInfo("", "", "", OPEN_ID, 1, 10));
        }
        [HttpGet]
        public PartialViewResult PartDailyCheckList(string status, string starttime, string endtime, int page, int limit)
        {
            string OPEN_ID = Request.HttpContext.User.Identity.Name;
            return PartialView("../DailyCheck/PartDailyCheckList", dailyCheckSvc.GetTaskInfo(status, starttime, OPEN_ID, endtime, page, limit));
            //return Ok(dailyCheckSvc.GetTaskInfo(status, starttime, endtime));
        }
        [HttpGet]
        public IActionResult DailyCheckDetail(string RWBH)
        {
            //string OPEN_ID=Request.HttpContext.User.Identity.Name;
            string OPEN_ID = "123123123123";
            return View(dailyCheckSvc.GetTaskDetailInfo(RWBH, OPEN_ID));
        }
        [HttpGet]
        public PartialViewResult PartDailyCheckDetail(string RWBH, int page, int limit)
        {
            //string OPEN_ID=Request.HttpContext.User.Identity.Name;
            string OPEN_ID = "123123123123";
            return PartialView(dailyCheckSvc.GetTaskDetailInfo(RWBH, OPEN_ID, page, limit));
        }

        public IActionResult CreateCheckResult()
        {
            return View();
        }
    }
}