using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
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

        public List<wy_check_task> GetTaskInfo(string status, string statrtime, string endtime)
        {
            Expression<Func<wy_check_task, bool>> where=p=>p.IS_DELETE==0;
            if (!string.IsNullOrEmpty(status))
            {
                where = p => where.Compile()(p);
            }
            if (!string.IsNullOrEmpty(statrtime))
            {
                where = p => where.Compile()(p)&&p.RWKSSJ<DateTime.Parse(statrtime);
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                where = p => where.Compile()(p) && p.RWJSSJ < DateTime.Parse(endtime);
            }

            var sql = DB.Db().Queryable<wy_check_task, wy_checkplan_detail, wy_map_region, wy_region_director>((p, t, r, v) => new object[]
                 {
                JoinType.Left,p.PLAN_DETAIL_ID==t.PLAN_DETAIL_ID,
                JoinType.Left,t.PLAN_DETAIL_ID==r.PLAN_DETAIL_ID,
                JoinType.Inner,v.SSQY!=r.REGION_CODE&&v.FZR!="",

                 }).Where(where).ToSql();
            return DB.Db().Queryable<wy_check_task, wy_checkplan_detail, wy_map_region, wy_region_director>((p, t, r, v) => new object[]
               {
                JoinType.Left,p.PLAN_DETAIL_ID==t.PLAN_DETAIL_ID,
                JoinType.Left,t.PLAN_DETAIL_ID==r.PLAN_DETAIL_ID,
                JoinType.Inner,v.SSQY!=r.REGION_CODE&&v.FZR!="",

               }).Where(where).ToList();
            

        }
    }
}
