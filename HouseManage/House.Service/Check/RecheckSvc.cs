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

        /***
         * 查询逻辑是检查结果通过两张映射表直接拿到检查区域，再去关联检查员信息，如果他的所属区域包含在任务范围内，此条任务可查看。
         * 复查和检查有一点不同的是检查是无论检查结果有没有不合格的都要查询出来，但是复查不一样，如果这个任务内他所负责的区域内所有的房子
         * 都是合格的，那么他应该卡不见这个任务，所以用exist内的子查询实现这个条件。
         ***/
        public List<wy_check_task> GetTaskInfo(string OPEN_ID, int page=1, int limit=10)
        {
            //var list = DB.Db().Queryable<wy_check_task, wy_map_checkplandetail, wy_checkplan_detail, wy_map_region, wy_region_director>
            //    ((a, b, c, d, e) => new object[]
            //    {
            //        JoinType.Inner,a.TASK_ID==b.TASK_ID,
            //        JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID&&c.IS_DELETE==0,
            //        JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
            //        JoinType.Inner,d.REGION_CODE==e.SSQY&&e.WX_OPEN_ID==OPEN_ID
            //    })
            //    .Where(a => a.IS_DELETE == 0)
            //    .GroupBy(a => new { a.TASK_ID })
            //    .Skip((page - 1) * limit).Take(limit).ToList();
           
            string sql = "SELECT DISTINCT a.* FROM wy_check_task a " +
                " JOIN wy_map_checkplandetail b ON a.TASK_ID=b.TASK_ID" +
                " JOIN wy_map_region c ON b.PLAN_DETAIL_ID=c.PLAN_DETAIL_ID" +
                " JOIN wy_region_director d ON c.REGION_CODE=d.SSQY AND d.IS_DELETE=0 AND d.WX_OPEN_ID='" + OPEN_ID + "'" +
                " WHERE a.IS_DELETE=0" +
                " AND EXISTS (" +
                " SELECT 1 FROM wy_check_result e" +
                " JOIN wy_houseinfo f ON e.FWID=f.FWID AND f.IS_DELETE=0" +
                " JOIN wy_region_director g ON f.SSQY=g.SSQY AND f.IS_DELETE=0 " +
                " WHERE e.TASK_ID=a.TASK_ID AND g.WX_OPEN_ID=d.WX_OPEN_ID AND e.JCJG!=1)" +
                " ORDER BY a.CJSJ";
            var list = DB.Db().SqlQueryable<wy_check_task>(sql).Skip((page - 1) * limit).Take(limit).ToList();
            return list;
        }
        /***
         * 查询逻辑如下：
         * 通过检查结果内保存的房屋ID直接去关联房屋表找到房屋的区域，再用区域关联检查人，这样就可以找到检查人所能查看到的房屋的检查结果
         * 然后通过房屋关联使用者，如果是转租用户则会使用转租者的信息作为商户信息。
         * 和检查不同的是，检查会查询所有的检查结果，复查只差不合格的。
         ***/
        public List<TaskListModel> GetTaskDetailInfo(string RWBH, string TASK_ID, string OPEN_ID, int page, int limit)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_houseinfo, wy_region_director, wy_shopinfo, wy_shopinfo>((a, b, c, d, e) => new object[] {
               JoinType.Inner,a.FWID==b.FWID&&b.IS_DELETE==0,
               JoinType.Inner,c.SSQY==b.SSQY&&c.IS_DELETE==0,
               JoinType.Inner,b.CZ_SHID==d.CZ_SHID&&d.IS_DELETE==0,
               JoinType.Left,e.CZ_SHID==d.SUBLET_ID&&d.IS_SUBLET==1
                })
                .Where((a, b, c, d, e) =>a.JCJG!=1&&a.TASK_ID == TASK_ID && c.WX_OPEN_ID == OPEN_ID)//和检查不一样的地方就是除了合格的都查
                .OrderBy(a => a.CJSJ, OrderByType.Desc)
                .GroupBy((a, b, c, d, e) => new { a.TASK_ID, a.RESULT_ID, a.JCJG, b.FWMC, b.FWBH, ZHXM = d.ZHXM, SHOP_NAME = d.SHOP_NAME, SHOPBH = d.SHOPBH, ZHXM1 = e.ZHXM, SHOP_NAME1 = e.SHOP_NAME, SHOPBH1 = e.SHOPBH })
                .Select((a, b, c, d, e) => new TaskListModel()
                {
                    TASK_ID = a.TASK_ID,
                    RESULT_ID = a.RESULT_ID,
                    JCJG = (int)a.JCJG,
                    FWBH = b.FWBH,
                    FWMC = b.FWMC,
                    ZHXM = e.ZHXM == null ? d.ZHXM : e.ZHXM,
                    SHOP_NAME = e.SHOP_NAME == null ? d.SHOP_NAME : e.SHOP_NAME,
                    SHOPBH = e.SHOPBH == null ? d.SHOPBH : e.SHOPBH,
                })
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
