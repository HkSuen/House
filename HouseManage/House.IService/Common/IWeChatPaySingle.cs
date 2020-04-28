using Data.MSSQL.Model.Data;
using House.IService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Common
{
    public interface IWeChatPaySingle
    {
        /// <summary>
        /// 生成统一下单支付接口
        /// 接口返回预付订单签名对象
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetPrepaySign(wy_wx_pay payModel);

        /// <summary>
        ///根据参数生成预付订单签名对象
        /// </summary>
        /// <param name="prePayId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetParamStrByPrePayId(string appId, string prePayId, string MchSecret,string OrderId,string Id);

        /// <summary>
        /// 检查签名
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        bool CheckWxSign(Dictionary<string, object> Dic);

        /// <summary>
        /// 根据订单号查询订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Dictionary<string, object> FindOrder(string appId, string orderId, string mchId);

        /// <summary>
        /// 请求微信检查订单支付状态
        /// </summary>
        /// <param name="orderParams"></param>
        /// <returns></returns>
        Dictionary<string, object> CheckOrder(Dictionary<string, object> orderParams);

    }
}
