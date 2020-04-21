using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///检查结果明细表
    ///</summary>
    public partial class wy_check_result_detail
    {
           public wy_check_result_detail(){


           }
           /// <summary>
           /// Desc:详情主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string CHECK_DETAIL_ID {get;set;}

           /// <summary>
           /// Desc:任务结果ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RESULT_ID {get;set;}

           /// <summary>
           /// Desc:任务详情code
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string DETAIL_CODE {get;set;}

           /// <summary>
           /// Desc:检查结果,0不合格 1合格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? CHECK_DETAIL_RESULT {get;set;}

           /// <summary>
           /// Desc:检查时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CHECK_DETAIL_TIME {get;set;}

           /// <summary>
           /// Desc:检查人
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string JCR {get;set;}

    }
}
