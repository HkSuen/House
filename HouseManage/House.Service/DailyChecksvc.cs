using Data.MSSQL;
using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using House.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace House.Service
{
    public class DailyCheckSvc: IDailyCheckSvc
    {
        private IDataConfig DB;
        public DailyCheckSvc(IDataConfig DB)
        {
            this.DB = DB;
        }

        public List<wy_check_task> GetTaskInfo(string status, string statrtime, string endtime,string OPEN_ID,int page,int limit)
        {
            var list = DB.Db().Queryable<wy_check_task, wy_checkplan_detail, wy_map_region, wy_region_director>((p, t, r, v) => new object[]
                {
                JoinType.Left,p.PLAN_DETAIL_ID==t.PLAN_DETAIL_ID,
                JoinType.Left,t.PLAN_DETAIL_ID==r.PLAN_DETAIL_ID,
                JoinType.Inner,v.SSQY==r.REGION_CODE&&v.FZR!="",

                })
                .WhereIF(!string.IsNullOrEmpty(statrtime), p => p.RWKSSJ >= DateTime.Parse(statrtime))
                .WhereIF(!string.IsNullOrEmpty(endtime), p => p.RWJSSJ <= DateTime.Parse(endtime))
                .Skip((page-1)*limit).Take(limit).ToList();
            return list;
        }

        public List<TaskListModel> GetTaskDetailInfo(string RWBH,string OPEN_ID,int page, int limit)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_houseinfo, wy_shopinfo>((a, b, c) => new object[] {
                JoinType.Left,a.FWID==b.FWID&&b.IS_DELETE==0,
                JoinType.Left,b.CZ_SHID==c.CZ_SHID&&c.IS_DELETE==0
            }).Where(a => a.TASK_ID ==SqlFunc.Subqueryable<wy_check_task>().Where(s=>s.RWBH==RWBH).Select(s=>s.TASK_ID)
            &&a.IS_DELETE==0&&a.JCR==OPEN_ID).
            Select<TaskListModel>()
            .Skip((page - 1) * limit).Take(limit).ToList();
            //var sql = DB.Db().Queryable<wy_check_result, wy_houseinfo, wy_shopinfo>((a, b, c) => new object[] {
            //    JoinType.Left,a.FWID==b.FWID&&b.IS_DELETE==0,
            //    JoinType.Left,b.CZ_SHID==c.CZ_SHID&&c.IS_DELETE==0
            //}).Where(a => a.TASK_ID == SqlFunc.Subqueryable<wy_check_task>().Where(s => s.RWBH == RWBH).Select(s => s.TASK_ID)
            //&& a.IS_DELETE == 0 && a.JCR == OPEN_ID).
            //Select<TaskListModel>()
            //.Skip((page - 1) * limit).Take(limit).ToSql();
            return list;
        }

    }
}
