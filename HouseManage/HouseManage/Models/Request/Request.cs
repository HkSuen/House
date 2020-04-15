using System;
using System.Collections.Generic;
using System.Text;

namespace HouseManage.Models.Request
{
    public class Request
    {
        public int? pageIndex { get; set; }
        public int? pageCount { get; set; }
        public int? pageLimit { get; set; }
        public Dictionary<string,string> conditions { get; set; }
    }
}
