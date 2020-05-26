using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Check
{
    public interface IDailyCheckSvc
    {
        /// <summary>
        /// 获取任务列表
        /// 查询逻辑：所有任务∩区域负责人所负责对应区域的任务
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
        /// 查询逻辑：较为简单 就是根据TASK_ID取出任务明细 同时关联房屋表和商户表。
        /// </summary>
        /// <param name="RWBH"></param>
        /// <param name="OPEN_ID"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        List<TaskListModel> GetTaskDetailInfo(string RWBH,string TASK_ID,string OPEN_ID,int page=1, int limit=10);
        /// <summary>
        /// 获取创建任务表单时所需要检查的明细列表
        /// 查询逻辑：任务内的年度计划详情的所有检查事项∩区域负责人所负责任务内的计划明细的检查事项∩所选房屋的检查事项
        /// </summary>
        /// <param name="RWBH"></param>
        /// <returns></returns>
        List<wy_task_detail_config> GetCreateTaskResultFormInfo(string FWID, string TASK_ID, string OPEN_ID);
        /// <summary>
        /// 获取任务可以检查的房屋列表
        /// 查询逻辑：任务详情内的年度计划明细内的区域的房子∩区域负责人所负责的区域的房子∩负责人区域未检查过的房子
        /// </summary>
        /// <param name="RWBH"></param>
        /// <returns></returns>
        List<SimpleShopInfo> GetShopInfo(string RWBH,string TASK_ID,string OPNE_ID);
        /// <summary>
        /// 提交检查结果
        /// </summary>
        /// <param name="d"></param>
        /// <param name="OPEN_ID"></param>
        /// <returns></returns>
        string PostCheckResult(Dictionary<string, object> d,string OPEN_ID, Action<bool, string, string, string, string, string, DateTime?> sendMsg = null);
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
        /// <summary>
        /// 提交修改信息
        /// </summary>
        /// <param name="d"></param>
        /// <param name="OPEN_ID"></param>
        /// <returns></returns>
        string PostUpdateCheckResult(Dictionary<string, object> d, string OPEN_ID);

        Dictionary<string,string> RWHBANDTask_ID(string RESULT_ID);
    }
}
