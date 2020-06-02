using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService;
using House.IService.Check;
using HouseManage.Common.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HouseManage.Controllers.Check
{
    [RolesAuthorize(Roles = new string[] { "Admin", "Inspector" })]
    public class DailyCheckController : ControllerBase
    {
        private IDailyCheckSvc dailyCheckSvc;
        //private string OpenID = "oAY4Pv4h8eyBSAs4O8psFw6omlsg";
        public DailyCheckController(IDailyCheckSvc dailyCheckSvc)
        {
            this.dailyCheckSvc = dailyCheckSvc;
        }
        public IActionResult Index()
        {
            return View(dailyCheckSvc.GetTaskInfo("", "", "", OpenID, 1, 10));
        }
        [HttpGet]
        public PartialViewResult PartDailyCheckList(string status, string starttime, string endtime, int page, int limit)
        {
           
            return PartialView("../DailyCheck/PartDailyCheckList", dailyCheckSvc.GetTaskInfo(status, starttime,endtime, OpenID, page, limit));

        }
        [HttpGet]
        public IActionResult DailyCheckDetail(string RWBH,string TASK_ID)
        {
            return View(dailyCheckSvc.GetTaskDetailInfo(RWBH,TASK_ID,OpenID));
        }
        [HttpGet]
        public PartialViewResult PartDailyCheckDetail(string RWBH,string TASK_ID,int page, int limit)
        {
            return PartialView(dailyCheckSvc.GetTaskDetailInfo(RWBH, TASK_ID,OpenID, page, limit));
        }

        public IActionResult CreateCheckResult(string RWBH,string TASK_ID)
        {
            return View();

        }

        [HttpGet]
        public IActionResult PartCreateCheckResult(string FWID,string TASK_ID)
        {
            return PartialView(dailyCheckSvc.GetCreateTaskResultFormInfo(FWID,TASK_ID,OpenID));
        }
        [HttpGet]
        public IActionResult GetShopInfo(string RWBH,string TASK_ID)
        {
            return Ok(dailyCheckSvc.GetShopInfo(RWBH, TASK_ID, OpenID));
        }
        [HttpPost]
        public IActionResult PostCheckResult([FromBody]JObject value)
        {
            return Ok(dailyCheckSvc.PostCheckResult(value.ToObject<Dictionary<string, object>>(), OpenID));
        }

        public IActionResult EditCheckResult(string RESULT_ID)
        {
            Dictionary<string, string> d = dailyCheckSvc.RWHBANDTask_ID(RESULT_ID);
            if (d == null)
                return View();
            ViewBag.RWBH = d["RWBH"];
            ViewBag.TASK_ID = d["TASK_ID"];
            ViewBag.IMGStr = d["IMGS"];
            bool ImgSte = d.TryGetValue("IMGS",out string IMGS);
            if (ImgSte && !string.IsNullOrEmpty(IMGS))
            {
                ViewBag.IMGS = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(IMGS);
            }
            else
            {
                ViewBag.IMGS = new string[0];
            }
            ViewBag.FWINFO= dailyCheckSvc.GetEditShopInfo(RESULT_ID);
            ViewBag.CHECKINFO = dailyCheckSvc.GetEditTaskResultFormInfo(RESULT_ID);
            return View();
        }

        [HttpPost]
        public IActionResult PostUpdateCheckResult([FromBody]JObject value)
        {
            return Ok(dailyCheckSvc.PostUpdateCheckResult(value.ToObject<Dictionary<string, object>>(), OpenID));
        }
    }
}