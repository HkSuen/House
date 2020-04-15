using HouseManage.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseManage.Models.Request
{
    public class Response
    {
        public ResultCode code { get; set; }
        public object data { get; set; }
        public string msg { get; set; }
    }
}
