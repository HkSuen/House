using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService
{
    public interface IDailyCheckSvc
    {
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="statrtime"></param>
        /// <param name="endtime"></param>
        /// <param name="OPEN_ID"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        List<wy_check_task> GetTaskInfo(string status, string statrtime,string endtime, string OPEN_ID, int page, int limit);
        /// <summary>
        /// 获取任务详情列表
        /// </summary>
        /// <param name="RWBH"></param>
        /// <param name="OPEN_ID"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        List<TaskListModel> GetTaskDetailInfo(string RWBH,string OPEN_ID,int page=1, int limit=10);
        /// <summary>
        /// 获取创建任务表单时所需要检查的明细列表
        /// </summary>
        /// <param name="RWBH"></param>
        /// <returns></returns>
        List<wy_task_detail_config> GetCreateTaskResultFormInfo(string RWBH);
        /// <summary>
        /// 获取任务可以检查的房屋列表
        /// </summary>
        /// <param name="RWBH"></param>
        /// <returns></returns>
        List<SimpleShopInfo> GetShopInfo(string RWBH);
        /// <summary>
        /// 提交检查结果
        /// </summary>
        /// <param name="d"></param>
        /// <param name="OPEN_ID"></param>
        /// <returns></returns>
        string PostCheckResult(Dictionary<string, object> d,string OPEN_ID);
        /// <summary>
        /// 获取修改页面的商户信息
        /// </summary>
        /// <param name="RESULT_ID"></param>
        /// <returns></returns>
        List<SimpleShopInfo> GetEditShopInfo(string RESULT_ID);
        /// <summary>
        /// 获取修改页面的所需要的检查明细
        /// 因为检查结果内可能有的检查项没有填，存的时候没有数据，所以不能直接从结果明细表获取。
        /// 还是需要通过年度计划明细关联出检查类型，再关联任务明细配置表获取任务自带的检查明细，在关联检查结果表。
        /// </summary>
        /// <param name="RESULT_ID"></param>
        /// <returns></returns>
        List<SimpleCheckResultDetail> GetEditTaskResultFormInfo(string RESULT_ID);
    }
}
