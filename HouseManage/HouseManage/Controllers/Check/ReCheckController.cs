using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Check;
using HouseManage.Common.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HouseManage.Controllers.Check
{
    [RolesAuthorize(Roles = new string[] { "Admin", "Inspector" })]
    
    public class ReCheckController : ControllerBase
    {
        //private new string OpenID = "oAY4PvyhwmlHhue1GwBdwsW-mmfI";
        private IRecheckSvc recheckSvc;
        public ReCheckController(IRecheckSvc recheckSvc)
        {
            this.recheckSvc = recheckSvc;
        }
        public IActionResult Index()
        {   
            return View(recheckSvc.GetTaskInfo(OpenID, 1, 10));
        }

        [HttpGet]
        public PartialViewResult PartRecheckList(int page, int limit)
        {
            return PartialView("../ReCheck/PartRecheckList", recheckSvc.GetTaskInfo(OpenID,page, limit));
        }

        [HttpGet]
        public IActionResult ReCheckDetail(string TASK_ID)
        {
            return View(recheckSvc.GetTaskDetailInfo("", TASK_ID, OpenID));
        }
        [HttpGet]
        public PartialViewResult PartReCheckDetail(string RWBH, string TASK_ID, int page, int limit)
        {
            return PartialView(recheckSvc.GetTaskDetailInfo(RWBH, TASK_ID, OpenID, page, limit));
        }

        public IActionResult ReCheckResult(string RESULT_ID)
        {
            return View(recheckSvc.GetReCheckForm(RESULT_ID));
        }

        [HttpPost]
        public IActionResult PostUpdateReCheckResult([FromBody]JObject value) => Ok(recheckSvc.PostUpdateReCheckResult(value.ToObject<Dictionary<string, object>>()));

    }
}