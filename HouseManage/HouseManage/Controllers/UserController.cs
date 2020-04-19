using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Model.Enum;
using House.IService.Users;
using HouseManage.Models.Enum;
using HouseManage.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using House.IService.Common;
using Microsoft.AspNetCore.Identity;

namespace HouseManage.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private IUsersSvc _user = null;

        public UserController(IUsersSvc users)
        {
            this._user = users;
        }


        public ActionResult Register(string uid, string redirect)
        {
            ViewBag.UID = Request.Query["uid"].FirstOrDefault();
            ViewBag.REDIRECT = Request.Query["redirect"].FirstOrDefault();
            return View();
        }

        [HttpPost]
        public JsonResult GetUserType()
        {
            return Json(UserRole.Role);
        }

        [HttpPost]
        public JsonResult WXRegister(string phone, string type, string uid)
        {
            int State = this._user.WXRegister(phone, type, uid);
            ResultCode Code = State == 1 ? ResultCode.SCCUESS : (State == 2 ? ResultCode.DATA_IS_WRONG : (State == 3 ? ResultCode.DATA_NOT_FOUND : ResultCode.PARAMS_TYPE_ERROR));
            var response = new Response()
            {
                code = Code,
                msg = Code.GetEnumDescription()
            };
            return Json(response);
        }
    }
}