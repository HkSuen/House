﻿using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///
    ///</summary>
    public partial class wy_check_result
    {
           public wy_check_result(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RESULT_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string TASK_ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LXDH {get;set;}

           /// <summary>
           /// Desc:检查结果 0不合格 1合格 2复查合格 3复查不合格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? JCJG {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string JCR {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? JCSJ {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string JCQK {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string JCTP {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CJR {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CJSJ {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string BJR {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? BJSJ {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? IS_DELETE {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FWID {get;set;}

           /// <summary>
           /// Desc:整改要求
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ZGYQ {get;set;}

           /// <summary>
           /// Desc:问题描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WTMS {get;set;}

           /// <summary>
           /// Desc:复查次数
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? JCCS {get;set;}

           /// <summary>
           /// Desc:是否反馈,0未反馈，1反馈
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? IS_REVIEW {get;set;}

           /// <summary>
           /// Desc:问题图片
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IMGS {get;set;}

    }
}
