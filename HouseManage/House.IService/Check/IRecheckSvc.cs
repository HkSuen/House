using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Check
{
    public interface IRecheckSvc
    {
        List<wy_check_task> GetTaskInfo(string OPEN_ID, int page=1, int limit=10);

        List<TaskListModel> GetTaskDetailInfo(string RWBH, string TASK_ID, string OPEN_ID, int page=1, int limit=10);

        List<ReCheckResultModel> GetReCheckForm(string RESULT_ID);

        bool PostUpdateReCheckResult(Dictionary<string, object> d);
    }
}
