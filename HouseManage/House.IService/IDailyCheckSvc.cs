using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService
{
    public interface IDailyCheckSvc
    {
        List<wy_check_task> GetTaskInfo(string status, string statrtime,string endtime, string OPEN_ID, int page, int limit);

        List<TaskListModel> GetTaskDetailInfo(string RWBH,string OPEN_ID,int page=1, int limit=10);
    }
}
