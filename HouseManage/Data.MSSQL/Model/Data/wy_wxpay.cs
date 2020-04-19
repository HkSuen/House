using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///
    ///</summary>
    public partial class wy_wxpay
    {
           public wy_wxpay(){


           }
           /// <summary>
           /// Desc:订单主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ID {get;set;}

           /// <summary>
           /// Desc:缴费记录的主键
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RECORD_ID {get;set;}

           /// <summary>
           /// Desc:房屋ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string HOUSE_ID {get;set;}

           /// <summary>
           /// Desc:商户ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_ID {get;set;}

           /// <summary>
           /// Desc:用户的缴费IP
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_IP {get;set;}

           /// <summary>
           /// Desc:OPENID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string OPEN_ID {get;set;}

           /// <summary>
           /// Desc:缴费类型 备注:0物业费 1水费 2 电费
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int FEE_TYPES {get;set;}

           /// <summary>
           /// Desc:总金额
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TOTAL_FEE {get;set;}

           /// <summary>
           /// Desc:订单状态：0订单生成，1订单超时，2支付成功，3支付失败
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int STATUS {get;set;}

           /// <summary>
           /// Desc:订单返回状态说明
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string STATUS_REMARK {get;set;}

           /// <summary>
           /// Desc:订单生成时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime CREATE_TIME {get;set;}

           /// <summary>
           /// Desc:订单支付时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime PAY_TIME {get;set;}

           /// <summary>
           /// Desc:订单ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ORDER_ID {get;set;}

           /// <summary>
           /// Desc:程序栈ID：公众号appid
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string APP_ID {get;set;}

           /// <summary>
           /// Desc:接收支付商户的ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MECH_ID {get;set;}

           /// <summary>
           /// Desc:交易类型
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TRADE_TYPE {get;set;}

           /// <summary>
           /// Desc:预支付订单ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string PREPAYID {get;set;}

           /// <summary>
           /// Desc:预支付订单生成时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime PREPAY_TIME {get;set;}

           /// <summary>
           /// Desc:预支付订单有效时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime PREPAY_ENDTIME {get;set;}

    }
}
