using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService
{
    public interface IDailyCheckSvc
    {
        List<wy_check_task> GetTaskInfo(string status, string statrtime, string endtime);
    }
}
