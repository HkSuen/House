using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///
    ///</summary>
    public partial class wy_ele_recharge
    {
           public wy_ele_recharge(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string address {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string cid {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string opr_id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string account_id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? count {get;set;}

           /// <summary>
           /// Desc:物业向电接口充值金额
           /// Default:
           /// Nullable:True
           /// </summary>           
           public double? Cost {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateDate {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CStatus {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CMessage {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Pstatus {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Pmessage {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? PUpdateDate {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CUpdateDate {get;set;}

           /// <summary>
           /// Desc:开户状态 0 为开户 1 开户
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? KaiHu {get;set;}

           /// <summary>
           /// Desc:清零状态 0未清零1 清零
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? QingLing {get;set;}

           /// <summary>
           /// Desc:开户失败原因
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string KaiHuMsg {get;set;}

           /// <summary>
           /// Desc:清零失败原因
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string QingLingMsg {get;set;}

           /// <summary>
           /// Desc:物业收取商户电金额
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? WYCost {get;set;}

           /// <summary>
           /// Desc:充值度数
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? EleAmount {get;set;}

    }
}
