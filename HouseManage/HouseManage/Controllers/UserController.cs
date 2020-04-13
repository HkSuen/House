using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.IService.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseManage.Controllers
{
    public class UserController : Controller
    {
        private IPeopleSvc _people = null;
        public UserController(IPeopleSvc peopleSvc)
        {
            this._people = peopleSvc;
        }

        // GET: User
        public JsonResult Index()
        {
            var List = _people.GetAll();
            return Json(List);
        }
    }
}