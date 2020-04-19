using System;
using System.Collections.Generic;
using System.Text;

namespace HouseManage.Models.Request
{
    public class Request
    {
        public SqlSugar.PageModel Page { get; set; }
        public Dictionary<string,string> Conditions { get; set; }
    }
}
