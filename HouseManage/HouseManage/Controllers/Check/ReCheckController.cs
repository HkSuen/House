using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Check;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HouseManage.Controllers.Check
{
    [AllowAnonymous]
    public class ReCheckController : Controller
    {
        private IRecheckSvc recheckSvc;
        public ReCheckController(IRecheckSvc recheckSvc)
        {
            this.recheckSvc = recheckSvc;
        }
        public IActionResult Index()
        {
            string OPEN_ID = "123123123123";       
            return View(recheckSvc.GetTaskInfo(OPEN_ID, 1, 10));
        }

        [HttpGet]
        public PartialViewResult PartRecheckList(int page, int limit)
        {
            //string OPEN_ID = Request.HttpContext.User.Identity.Name;
            string OPEN_ID = "123123123123";
            return PartialView("../ReCheck/PartRecheckList", recheckSvc.GetTaskInfo(OPEN_ID,page, limit));
        }

        [HttpGet]
        public IActionResult ReCheckDetail(string TASK_ID)
        {
            //string OPEN_ID=Request.HttpContext.User.Identity.Name;
            string OPEN_ID = "123123123123";
            return View(recheckSvc.GetTaskDetailInfo("", TASK_ID, OPEN_ID));
        }
        [HttpGet]
        public PartialViewResult PartReCheckDetail(string RWBH, string TASK_ID, int page, int limit)
        {
            //string OPEN_ID=Request.HttpContext.User.Identity.Name;
            string OPEN_ID = "123123123123";
            return PartialView(recheckSvc.GetTaskDetailInfo(RWBH, TASK_ID, OPEN_ID, page, limit));
        }

        public IActionResult ReCheckResult(string RESULT_ID)
        {
            return View(recheckSvc.GetReCheckForm(RESULT_ID));
        }

        [HttpPost]
        public IActionResult PostUpdateReCheckResult([FromBody]JObject value) => Ok(recheckSvc.PostUpdateReCheckResult(value.ToObject<Dictionary<string, object>>()));

    }
}