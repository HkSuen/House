using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseManage.Controllers
{
    public class ControllerBase : Controller
    {
        //protected string OpenID => Request.HttpContext.User.Identity.Name;
        protected string OpenID => "123";
    }
}
