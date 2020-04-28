using Data.MSSQL.Common;
using House.IService.Common;
using House.IService.Model.Enum;
using HouseManage.Models.Enum;
using HouseManage.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseManage.Controllers
{
    public class ControllerBase : Controller
    {
        #region infomation of user.
        protected string OpenID => Request.HttpContext.User.Identity.Name;
        protected string UserID => Request.HttpContext.User.FindFirst("Uid")?.Value;
        protected URole UserRole;
        protected string UserIP
        {
            get
            {
                if (Request.Headers.ContainsKey("X-Real-IP"))
                {
                    return Request.Headers["X_Real-IP"].ToString();
                }
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return Request.Headers["X-Forwarded-For"].ToString();
                }
                return "0.0.0.0";
            }
        }
        #endregion
        protected JsonResult OK(object data)
        {
            return Data(ResultCode.SCCUESS, data);
        }

        protected JsonResult Data(ResultCode code, object data = null, string msg = null)
        {
            return Json(new Response() { code = code, data = data, msg = msg });
        }

        protected ActionResult Error(string msg = "Abnormal access!")
        {
            return Redirect($"/WeChat/Home/Error?msg={msg}");
        }

        protected ActionResult Exception(string msg = "Abnormal access!")
        {
            throw new Exception(msg);
        }


    }
}
