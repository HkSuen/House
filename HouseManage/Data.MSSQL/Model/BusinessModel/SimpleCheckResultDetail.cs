using System;
using System.Collections.Generic;
using System.Text;

namespace Data.MSSQL.Model.BusinessModel
{
    public class SimpleCheckResultDetail
    {
        public string Code { get; set; }

        public string Name { get; set; }
        public int? CHECK_DETAIL_RESULT { get; set; }
        public string CHECK_RESULT_ID { get; set; }
    }
}
