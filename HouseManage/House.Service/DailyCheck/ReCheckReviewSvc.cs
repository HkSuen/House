using Data.MSSQL;
using Data.MSSQL.Model.Data;
using House.IService;
using House.IService.Common.AppSetting;
using House.IService.Common.Message;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.Service.DailyCheck
{
    public class ReCheckReviewSvc : IReCheckReviewSvc
    {
        private IDataConfig _Db = null;
        public ReCheckReviewSvc(IDataConfig dataConfig)
        {
            this._Db = dataConfig;
        }
        public Dictionary<string, object> GetRecheckReviewData(string openId) {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var list = _Db.Db().Queryable<wy_shopinfo, wy_houseinfo, wy_check_result, wy_check_task>((a, b, c, d) => new object[]{
                     JoinType.Inner,a.CZ_SHID==b.CZ_SHID&&a.OPEN_ID==openId&&b.IS_DELETE==0&&a.IS_DELETE==0,
                     JoinType.Inner,c.FWID==b.FWID&&c.JCJG!=1&&c.IS_REVIEW!=1,
                     JoinType.Inner,d.TASK_ID==c.TASK_ID
                }).OrderBy((a,b,c,d)=>c.CJSJ, OrderByType.Desc).Select((a, b, c, d) =>new { b.FWBH,b.FWMC,d.RWBH, d.RWMC,c.CJSJ ,c.RESULT_ID,c.CJR}).ToList();
                 dic.Add("list", list);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        public Dictionary<string, object> GetRecheckReviewDataDetail(string resultid)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var list = _Db.Db().Queryable<wy_check_result, wy_check_result_detail, wy_task_detail_config>((a, b, c) => new object[]{
                     JoinType.Inner,a.RESULT_ID==b.RESULT_ID&&a.RESULT_ID==resultid,
                     JoinType.Inner,c.Code==b.DETAIL_CODE&&c.ParentID!=null
                }).Select((a, b, c) => new { a.WTMS, b.CHECK_DETAIL_RESULT, checkName=c.Name  }).ToList();
                dic.Add("list", list);
            }
            catch (Exception ex)
            {
                dic.Add("list", null);
                return dic;
            }
            return dic;
        }
        public  string ReviewCheckConfirm(string resultId, string rwbh, string fwbh, string fwmc, string jcr_openid, string rwmc) {
            string reslult = "";
            try
            {
                wy_check_result wcr = new wy_check_result()
                {
                    RESULT_ID = resultId,
                    IS_REVIEW = 1
                };
                int res = _Db.Db().Updateable(wcr).UpdateColumns(a=>new { a.IS_REVIEW}).WhereColumns(it => it.RESULT_ID).ExecuteCommand();
                if (res > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("first","您收到一个商户房屋整改反馈通知：");
                    dic.Add("keyword1", rwbh);
                    dic.Add("keyword2",rwmc);
                    dic.Add("keyword3", fwbh);
                    dic.Add("keyword4", fwmc);
                    string msgTempId = AppSetting.GetSection("msgtemp:temp");
                    string url = AppSetting.GetSection("msgUrl:url");
                    MsgHelper.Msg.SendMsg(url, jcr_openid, dic,msgTempId);
                    return reslult;
                }
                else
                {
                    return "提交失败！";
                }
            }
            catch (Exception ex)
            {
                reslult = ex.ToString();
                return reslult;
            }
          
        }


    }
}
