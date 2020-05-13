using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers
{
    //[Authorize(Roles = "Merchant")]
    [AllowAnonymous]
    public class ReCheckReviewController : ControllerBase
    {
        private IReCheckReviewSvc _recheck = null;
        public ReCheckReviewController(IReCheckReviewSvc svc)
        {
            _recheck = svc;
        }
        /// <summary>
        /// 检查反馈页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询不合格任务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetRecheckReviewData()
        {
            //string OpenIDs = OpenID;
            string OpenIDs = "oAY4Pv8qLZLMySKDoKrv-Zz1xBZ0";
            return Json(_recheck.GetRecheckReviewData(OpenIDs));
        }
        /// <summary>
        /// 查询单条任务检查明细
        /// </summary>
        /// <param name="resultId"></param>
        /// <param name="rwbh"></param>
        /// <param name="fwbh"></param>
        /// <param name="fwmc"></param>
        /// <param name="jcr_openid">string resultId, string rwbh, string fwbh, string fwmc,string jcr_openid,string rwmc</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ReviewCheckDetail(string resultId, string rwbh, string fwbh, string fwmc, string jcr_openid, string rwmc)
        {
            ViewBag.resultId = resultId;
            ViewBag.rwbh = rwbh;
            ViewBag.rwmc = rwmc;
            ViewBag.fwbh = fwbh;
            ViewBag.fwmc = fwmc;
            ViewBag.jcr_openid = jcr_openid;
            return View();
        }
        /// <summary>
        /// 查询单条任务检查明细
        /// </summary>
        /// <param name="resultid"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetRecheckReviewDataDetail(string resultid)
        {
            return Json(_recheck.GetRecheckReviewDataDetail(resultid));
        }
        [HttpGet]
        public IActionResult ReviewCheckConfirm(string resultId, string rwbh, string fwbh, string fwmc, string jcr_openid,string rwmc) {

            return Content(_recheck.ReviewCheckConfirm(resultId,  rwbh,  fwbh,  fwmc,  jcr_openid,rwmc));
        }
    }
}