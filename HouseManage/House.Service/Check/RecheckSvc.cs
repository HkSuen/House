using Data.MSSQL;
using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using House.IService.Check;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.Service.Check
{
    public class RecheckSvc : IRecheckSvc
    {
        private IDataConfig DB;
        public RecheckSvc(IDataConfig DB)
        {
            this.DB = DB;
        }

        public List<wy_check_task> GetTaskInfo(string OPEN_ID, int page=1, int limit=10)
        {
            var list = DB.Db().Queryable<wy_check_task, wy_map_checkplandetail, wy_checkplan_detail, wy_map_region, wy_region_director>
                ((a, b, c, d, e) => new object[]
                {
                    JoinType.Inner,a.TASK_ID==b.TASK_ID,
                    JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID&&c.IS_DELETE==0,
                    JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
                    JoinType.Inner,d.REGION_CODE==e.SSQY&&e.WX_OPEN_ID==OPEN_ID
                })
                .Where(a => a.IS_DELETE == 0)
                .GroupBy(a => new { a.TASK_ID })
                .Skip((page - 1) * limit).Take(limit).ToList();
            return list;
        }

        public List<TaskListModel> GetTaskDetailInfo(string RWBH, string TASK_ID, string OPEN_ID, int page, int limit)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_houseinfo, wy_shopinfo, wy_map_checkplandetail, wy_map_region, wy_region_director>
                ((a, b, c, d, e, f) => new object[] {
                JoinType.Left,a.FWID==b.FWID,
                JoinType.Left,b.CZ_SHID==c.CZ_SHID,
                JoinType.Inner,a.TASK_ID==d.TASK_ID,
                JoinType.Inner,d.PLAN_DETAIL_ID==e.PLAN_DETAIL_ID,
                JoinType.Inner,f.SSQY==e.REGION_CODE
            })
                .Where((a, b, c, d, e, f) => a.JCJG==0&&b.IS_DELETE == 0 && c.IS_DELETE == 0 && a.TASK_ID == TASK_ID && f.IS_DELETE == 0 && f.WX_OPEN_ID == OPEN_ID
            //    a.TASK_ID ==SqlFunc.Subqueryable<wy_check_task>().Where(s=>s.RWBH==RWBH).Select(s=>s.TASK_ID)
            //&&a.IS_DELETE==0&&a.JCR==OPEN_ID
            )
                .OrderBy((a, b, c) => new { a.CJSJ }, OrderByType.Desc)
                .Select<TaskListModel>()
                .Skip((page - 1) * limit).Take(limit).ToList();
            return list;
        }

        public List<ReCheckResultModel> GetReCheckForm(string RESULT_ID)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_check_result_detail, wy_houseinfo, wy_task_detail_config>((a, b, c, d) => new object[]
              {
                JoinType.Inner,a.RESULT_ID==b.RESULT_ID&&b.CHECK_DETAIL_RESULT==0,
                JoinType.Inner,a.FWID==c.FWID&&c.IS_DELETE==0,
                JoinType.Inner,b.DETAIL_CODE==d.Code&&d.ParentID!=null
              })
                .Select((a, b, c, d) => new ReCheckResultModel()
                {
                    RESULT_ID = a.RESULT_ID,
                    TASK_ID = a.TASK_ID,
                    ZGYQ = a.ZGYQ,
                    WTMS = a.WTMS,
                    DETAIL_CODE = b.DETAIL_CODE,
                    CHECK_DETAIL_RESULT = b.CHECK_DETAIL_RESULT,
                    NAME = d.Name,
                    FWBH = c.FWBH,
                    FWMC = c.FWMC,
                    ZLWZ = c.ZLWZ
                })
                .Where(a => a.RESULT_ID == RESULT_ID).ToList();
            return list;
        }

        public bool PostUpdateReCheckResult(Dictionary<string, object> d)
        {
            string partSql = string.Empty;
            if (d["JCJG"].ToString() == "3")
            {
                partSql = ",IS_REVIEW=0";
            }
            string sql = "UPDATE wy_check_result SET JCJG=" + d["JCJG"] + ",JCCS=JCCS+1 {0} where RESULT_ID='" + d["RESULT_ID"] + "'";
            sql = string.Format(sql, partSql);
            return DB.Sql().ExecuteCommand(sql) > 0;
        }
    }
}
