using System;
using System.Collections.Generic;
using System.Text;

namespace Data.MSSQL.Model.BusinessModel
{
    public class ReCheckResultModel
    {
        public string RESULT_ID { get; set; }

        public string TASK_ID { get; set; }
        public string ZGYQ { get; set; }
        public string WTMS { get; set; }
        public string DETAIL_CODE { get; set; }
        public int? CHECK_DETAIL_RESULT { get; set; }
        public string NAME { get; set; }
        public string FWBH { get; set; }
        public string FWMC { get; set; }
        public string ZLWZ { get; set; }
    }
}
