using System;
using System.Linq;
using System.Text;

namespace Data.MSSQL.Model.Data
{
    ///<summary>
    ///
    ///</summary>
    public partial class wy_wx_pay
    {
           public wy_wx_pay(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ID {get;set;}

           /// <summary>
           /// Desc:订单ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ORDER_ID {get;set;}

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
           /// Desc:房屋名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string HOUSE_NAME {get;set;}

           /// <summary>
           /// Desc:房屋位置
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string HOUSE_ADDRESS {get;set;}

           /// <summary>
           /// Desc:房屋的面积
           /// Default:
           /// Nullable:True
           /// </summary>           
           public double? HOUSE_AREA {get;set;}

           /// <summary>
           /// Desc:商户ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_ID {get;set;}

           /// <summary>
           /// Desc:商户名字
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_NAME {get;set;}

           /// <summary>
           /// Desc:用户的缴费IP
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string USER_IP {get;set;}

           /// <summary>
           /// Desc:商铺名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string SHOP_NAME {get;set;}

           /// <summary>
           /// Desc:OPENID支付用户标识
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
           /// Desc:单个商品单价(分)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? UNIT_PRICE {get;set;}

           /// <summary>
           /// Desc:商品的数量
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? AMOUNT {get;set;}

           /// <summary>
           /// Desc:总金额(单位分)
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int TOTAL_FEE {get;set;}

           /// <summary>
           /// Desc:大写的总金额
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TOTAL_FEE_CH {get;set;}

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
           /// Nullable:True
           /// </summary>           
           public DateTime? PAY_TIME {get;set;}

           /// <summary>
           /// Desc:程序栈ID：公众号appid,也可以做其他的标识
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
           /// Desc:支付商户的名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string MECH_NAME {get;set;}

           /// <summary>
           /// Desc:交易类型 JSAPI
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TRADE_TYPE {get;set;}

           /// <summary>
           /// Desc:预支付订单ID
           /// Default:
           /// Nullable:True
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

           /// <summary>
           /// Desc:订单说明
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string REMARK {get;set;}

           /// <summary>
           /// Desc:随机字符串
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string NONCE_STR {get;set;}

    }
}
