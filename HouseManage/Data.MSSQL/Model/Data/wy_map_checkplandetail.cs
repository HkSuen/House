using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///任务 年度计划详情映射表
    ///</summary>
    public partial class wy_map_checkplandetail
    {
           public wy_map_checkplandetail(){


           }
           /// <summary>
           /// Desc:任务ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TASK_ID {get;set;}

           /// <summary>
           /// Desc:计划详情ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PLAN_DETAIL_ID {get;set;}

    }
}
