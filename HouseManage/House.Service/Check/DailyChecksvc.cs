﻿using Data.MSSQL;
using Data.MSSQL.Model.BusinessModel;
using Data.MSSQL.Model.Data;
using House.IService;
using House.IService.Check;
using Newtonsoft.Json.Linq;
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
            var list = DB.Db().Queryable<wy_check_task,wy_map_checkplandetail,wy_checkplan_detail, wy_map_region, wy_region_director>
                ((a,b,c,d,e) => new object[]
                {
                    JoinType.Inner,a.TASK_ID==b.TASK_ID,
                    JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID&&c.IS_DELETE==0,
                    JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
                    JoinType.Inner,d.REGION_CODE==e.SSQY&&e.WX_OPEN_ID==OPEN_ID
                })
                .WhereIF(!string.IsNullOrEmpty(statrtime), a => a.RWKSSJ >= DateTime.Parse(statrtime))
                .WhereIF(!string.IsNullOrEmpty(endtime), a => a.RWJSSJ <= DateTime.Parse(endtime))
                .Where(a=>a.IS_DELETE==0)
                .GroupBy(a => new { a.TASK_ID })
                .Skip((page-1)*limit).Take(limit).ToList();
            return list;
        }

        public List<TaskListModel> GetTaskDetailInfo(string RWBH,string TASK_ID,string OPEN_ID,int page, int limit)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_houseinfo, wy_shopinfo>((a, b, c) => new object[] {
                JoinType.Left,a.FWID==b.FWID,
                JoinType.Left,b.CZ_SHID==c.CZ_SHID
            })
                .Where((a,b,c) => b.IS_DELETE==0&&c.IS_DELETE==0&&a.TASK_ID==TASK_ID
            //    a.TASK_ID ==SqlFunc.Subqueryable<wy_check_task>().Where(s=>s.RWBH==RWBH).Select(s=>s.TASK_ID)
            //&&a.IS_DELETE==0&&a.JCR==OPEN_ID
            )
                .Select<TaskListModel>()
                .Skip((page - 1) * limit).Take(limit).ToList();
            return list;
        }

        public List<wy_task_detail_config> GetCreateTaskResultFormInfo(string FWID,string TASK_ID,string OPEN_ID)
        {
            //var list = DB.Db().Queryable<wy_check_task, wy_checkplan_detail, wy_task_detail_config, wy_task_detail_config>
            //    ((a, b, c, d) => new object[] {
            //    JoinType.Inner,b.PLAN_DETAIL_ID==a.PLAN_DETAIL_ID,
            //    JoinType.Inner,b.JCLX==c.Code,
            //    JoinType.Inner,c.ID==d.ParentID,
            //})
            //    .Where(a=>a.TASK_ID==TASK_ID)
            //    .Select((a, b, c, d) => new wy_task_detail_config()
            //{
            //    ID = d.ID,
            //    ParentID = d.ParentID,
            //    Name = d.Name,
            //    Code = d.Code
            //})
            //    .ToList();
            //return list;
            var list = DB.Db().Queryable
                <wy_check_task, wy_map_checkplandetail, wy_checkplan_detail, wy_map_region, wy_houseinfo, wy_task_detail_config, wy_task_detail_config, wy_region_director>
                ((a, b, c, d, e, f, g, h) => new object[] {
                    JoinType.Inner,a.TASK_ID==b.TASK_ID,
                    JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID&&c.IS_DELETE==0,
                    JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
                    JoinType.Inner,d.REGION_CODE==e.SSQY&&e.IS_DELETE==0,
                    JoinType.Inner,c.JCLX==f.Code,
                    JoinType.Inner,f.ID==g.ParentID,
                    JoinType.Inner,e.SSQY==h.SSQY&&h.IS_DELETE==0
                })
                .Where((a, b, c, d, e, f, g, h) => a.TASK_ID == TASK_ID && e.FWID == FWID && h.WX_OPEN_ID == OPEN_ID)
                .GroupBy((a, b, c, d, e, f, g, h)=>new { g.ID,g.ParentID,g.Name,g.Code})
                .Select((a, b, c, d, e, f, g, h) => new wy_task_detail_config()
                {
                    ID = g.ID,
                    ParentID = g.ParentID,
                    Name = g.Name,
                    Code = g.Code
                }).ToList();
            return list;
        }

        public List<SimpleShopInfo> GetShopInfo(string RWBH,string TASK_ID,string OPEN_ID)
        {
            //string sql = "select a.FWID,b.ZHXM,b.SHOP_NAME,b.SHOPBH from wy_houseinfo a " +
            //    " join wy_shopinfo b on a.CZ_SHID=b.CZ_SHID and b.IS_DELETE=0" +
            //    " join wy_map_region c on a.SSQY=c.REGION_CODE" +
            //    " join wy_check_task d on d.PLAN_DETAIL_ID=c.PLAN_DETAIL_ID" +
            //    " where a.IS_DELETE=0" +
            //    " AND a.FWID NOT IN " +
            //    " (SELECT FWID FROM wy_check_result WHERE TASK_ID=" +
            //    " (SELECT TASK_ID FROM wy_check_task WHERE RWBH='" + RWBH + "'))";
            //var list1 = DB.Db().SqlQueryable<SimpleShopInfo>(sql).ToList();
            var list = DB.Db().Queryable<wy_check_task, wy_map_checkplandetail, wy_checkplan_detail, wy_map_region, wy_region_director, wy_houseinfo, wy_shopinfo>
                ((a, b, c, d, e, f, g) => new object[]
            {
                JoinType.Inner,a.TASK_ID==b.TASK_ID,
                JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID,
                JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
                JoinType.Inner,d.REGION_CODE==e.SSQY&&e.IS_DELETE==0,
                JoinType.Inner,e.SSQY==f.SSQY&&f.IS_DELETE==0&&e.WX_OPEN_ID==OPEN_ID,
                JoinType.Inner,f.CZ_SHID==g.CZ_SHID&&g.IS_DELETE==0
            }).Where((a, b, c, d, e, f, g) => a.TASK_ID == TASK_ID
            && SqlFunc.Subqueryable<wy_check_result>().Where(t => t.TASK_ID == TASK_ID&&f.FWID==t.FWID).NotAny())
            .GroupBy((a, b, c, d, e, f, g) => new { f.FWID,g.ZHXM,g.SHOP_NAME,g.SHOPBH})
            .Select((a, b, c, d, e, f, g) => new SimpleShopInfo()
            {
                FWID = f.FWID,
                ZHXM = g.ZHXM,
                SHOP_NAME = g.SHOP_NAME,
                SHOPBH = g.SHOPBH
            }).ToList();

            //var sql = DB.Db().Queryable<wy_check_task, wy_map_checkplandetail, wy_checkplan_detail, wy_map_region, wy_region_director, wy_houseinfo, wy_shopinfo>
            //    ((a, b, c, d, e, f, g) => new object[]
            //{
            //    JoinType.Inner,a.TASK_ID==b.TASK_ID,
            //    JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID,
            //    JoinType.Inner,c.PLAN_DETAIL_ID==d.PLAN_DETAIL_ID,
            //    JoinType.Inner,d.REGION_CODE==e.SSQY&&e.IS_DELETE==0,
            //    JoinType.Inner,e.SSQY==f.SSQY&&f.IS_DELETE==0&&e.WX_OPEN_ID==OPEN_ID,
            //    JoinType.Inner,f.CZ_SHID==g.CZ_SHID&&g.IS_DELETE==0
            //}).Where((a, b, c, d, e, f, g) => a.TASK_ID == TASK_ID
            //&& SqlFunc.Subqueryable<wy_check_result>().Where(t => t.TASK_ID == TASK_ID && f.FWID == t.FWID).NotAny())
            //.Select((a, b, c, d, e, f, g) => new SimpleShopInfo()
            //{
            //    FWID = f.FWID,
            //    ZHXM = g.ZHXM,
            //    SHOP_NAME = g.SHOP_NAME,
            //    SHOPBH = g.SHOPBH
            //}).ToSql();
            return list;

        }

        public string PostCheckResult(Dictionary<string,object> d,string OPEN_ID)
        {
            wy_check_result wcr = new wy_check_result();
            Dictionary<string, object> ww = JObject.FromObject(d["RESULT_DETAIL"]).ToObject<Dictionary<string, object>>();
            string RESULT_ID = Guid.NewGuid().ToString();
            wcr.RESULT_ID = RESULT_ID;
            //wcr.TASK_ID = DB.Db().Queryable<wy_check_task>().Where(a => a.RWBH == d["RWBH"].ToString()).Select(a => a.TASK_ID).ToList()[0];
            wcr.TASK_ID = d["TASK_ID"].ToString();
            if (d["JCJG"].ToString() != "")
            {
                wcr.JCJG = Convert.ToInt32(d["JCJG"]);
            }
            wcr.JCR = OPEN_ID;
            wcr.JCSJ = DateTime.Now;
            wcr.ZGYQ = d["ZGYQ"].ToString();
            wcr.WTMS = d["ZGYQ"].ToString();
            wcr.FWID = d["FWID"].ToString();
            wcr.IS_DELETE = 0;
            wcr.JCCS = 0;
            wcr.IS_REVIEW = 0;
            List<wy_check_result_detail> list = new List<wy_check_result_detail>();
            foreach(KeyValuePair<string,object> item in ww)
            {
                wy_check_result_detail wcrd = new wy_check_result_detail();
                wcrd.CHECK_DETAIL_ID = Guid.NewGuid().ToString();
                wcrd.RESULT_ID = RESULT_ID;
                wcrd.DETAIL_CODE = item.Key;
                wcrd.CHECK_DETAIL_RESULT =Convert.ToInt32(item.Value);
                wcrd.CHECK_DETAIL_TIME = DateTime.Now;
                wcrd.JCR = OPEN_ID;
                list.Add(wcrd);
            }
            try
            {
                DB.Db().BeginTran();
                DB.Db().Insertable(wcr).ExecuteCommand();
                DB.Db().Insertable(list).ExecuteCommand();
                DB.Db().CommitTran();
            }
            catch(Exception e)
            {
                DB.Db().RollbackTran();
                return e.Message;
            }   
            return "success";
            
        }

       

        public List<SimpleShopInfo> GetEditShopInfo(string RESULT_ID)
        {
            var list = DB.Db().Queryable<wy_check_result, wy_houseinfo,wy_shopinfo>((a, b,c) => new object[]
            {
                JoinType.Inner,a.FWID==b.FWID,
                JoinType.Inner,b.CZ_SHID==c.CZ_SHID
            })
                .Where((a,b,c) => a.RESULT_ID == RESULT_ID&&b.IS_DELETE==0&&c.IS_DELETE==0)
                .Select<SimpleShopInfo>()
                .ToList();
            return list;
        }

        public List<SimpleCheckResultDetail> GetEditTaskResultFormInfo(string RESULT_ID)
        {
            //var list = DB.Db().Queryable<wy_check_task, wy_checkplan_detail, wy_task_detail_config, wy_task_detail_config, wy_check_result, wy_check_result_detail>
            //    ((a, b, c, d, e, f) => new object[]
            //  {
            //    JoinType.Inner,a.PLAN_DETAIL_ID==b.PLAN_DETAIL_ID,
            //    JoinType.Inner,b.JCLX==c.Code,
            //    JoinType.Inner,c.ID==d.ParentID,
            //    JoinType.Inner,e.TASK_ID==a.TASK_ID,
            //    JoinType.Left,d.Code==f.DETAIL_CODE&&f.RESULT_ID==RESULT_ID
            //  })
            //    .Where((a, b, c, d, e, f) => e.RESULT_ID == RESULT_ID)
            //    .Select((a, b, c, d, e, f) => new SimpleCheckResultDetail
            //    {
            //        Code = d.Code,
            //        Name = d.Name,
            //        CHECK_DETAIL_RESULT = f.CHECK_DETAIL_RESULT,
            //        CHECK_RESULT_ID = f.CHECK_DETAIL_ID,
            //    }).ToList();
            //return list;
            var list = DB.Db().Queryable<wy_check_task,wy_map_checkplandetail, wy_checkplan_detail, wy_task_detail_config, wy_task_detail_config, wy_check_result, wy_check_result_detail>
                ((a, b, c, d, e, f,g) => new object[]
              {
                JoinType.Inner,a.TASK_ID==b.TASK_ID,
                JoinType.Inner,b.PLAN_DETAIL_ID==c.PLAN_DETAIL_ID,
                JoinType.Inner,c.JCLX==d.Code,
                JoinType.Inner,d.ID==e.ParentID,
                JoinType.Inner,f.TASK_ID==a.TASK_ID,
                JoinType.Left,e.Code==g.DETAIL_CODE&&g.RESULT_ID==RESULT_ID
              })
                .Where((a, b, c, d, e, f, g) => f.RESULT_ID == RESULT_ID)
                .Select((a, b, c, d, e, f, g) => new SimpleCheckResultDetail
                {
                    Code = e.Code,
                    Name = e.Name,
                    CHECK_DETAIL_RESULT = g.CHECK_DETAIL_RESULT,
                    CHECK_RESULT_ID = g.CHECK_DETAIL_ID,
                }).ToList();
            return list;
        }

        public string PostUpdateCheckResult(Dictionary<string, object> d, string OPEN_ID)
        {
            try
            {
                wy_check_result wcr = new wy_check_result()
                {
                    RESULT_ID = d["RESULT_ID"].ToString(),
                    //FWID = d["FWID"].ToString(),
                    JCJG = Convert.ToInt32(d["JCJG"]),
                    WTMS = d["WTMS"].ToString(),
                    ZGYQ = d["ZGYQ"].ToString(),
                    BJR=OPEN_ID,
                    BJSJ=DateTime.Now
                };
                List<Dictionary<string, object>> updatelist = JArray.FromObject(d["CHECK_DETAIL_RESULT"]).ToObject<List<Dictionary<string, object>>>();
                List<wy_check_result_detail> list = new List<wy_check_result_detail>();
                foreach (Dictionary<string, object> dd in updatelist)
                {
                    wy_check_result_detail item = new wy_check_result_detail();
                    item.CHECK_DETAIL_ID = dd["CHECK_DETAIL_ID"].ToString();
                    item.CHECK_DETAIL_RESULT = Convert.ToInt32(dd["VALUE"]);
                    list.Add(item);
                }
                Dictionary<string, object> insertlist = JObject.FromObject(d["INSERTINFO"]).ToObject<Dictionary<string, object>>();
                List<wy_check_result_detail> list1 = new List<wy_check_result_detail>();
                foreach(KeyValuePair<string,object> kp in insertlist)
                {
                    wy_check_result_detail item1 = new wy_check_result_detail()
                    {
                        CHECK_DETAIL_ID = Guid.NewGuid().ToString(),
                        DETAIL_CODE = kp.Key,
                        CHECK_DETAIL_RESULT = Convert.ToInt32(kp.Value),
                        CHECK_DETAIL_TIME = DateTime.Now,
                        JCR = OPEN_ID,
                        RESULT_ID = d["RESULT_ID"].ToString()     
                    };
                    list1.Add(item1);
                }
                DB.Db().BeginTran();
                DB.Db().Updateable(wcr).IgnoreColumns(it => new { it.TASK_ID, it.FWID,it.CJR,it.CJSJ,it.IS_DELETE,it.JCCS,it.IS_REVIEW }).ExecuteCommand();
                DB.Db().Updateable(list).IgnoreColumns(it => new { it.DETAIL_CODE,it.JCR,it.RESULT_ID }).ExecuteCommand();
                DB.Db().Insertable(list1).ExecuteCommand();
                DB.Db().CommitTran();
            }
            catch(Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        public Dictionary<string,string> RWHBANDTask_ID(string RESULT_ID)
        {
            var item = DB.Db().Queryable<wy_check_result, wy_check_task>((a, b) => new object[]
              {
                JoinType.Inner,a.TASK_ID==b.TASK_ID&&b.IS_DELETE==0
              }).Where((a, b) => a.RESULT_ID == RESULT_ID).Select((a, b) => new
              {
                  TASK_ID = b.TASK_ID,
                  RWBH = b.RWBH
              }).First();
            Dictionary<string, string> d = new Dictionary<string, string>()
            {
                {"TASK_ID",item.TASK_ID },
                {"RWBH",item.RWBH }
            };
            return d;
        }
    }
}