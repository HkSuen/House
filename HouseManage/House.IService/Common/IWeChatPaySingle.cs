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
        Dictionary<string, object> GetPrepaySign(wy_wxpay payModel);

        /// <summary>
        ///根据参数生成预付订单签名对象
        /// </summary>
        /// <param name="prePayId"></param>
        /// <returns></returns>
        Dictionary<string, object> GetParamStrByPrePayId(string appId, string prePayId, string MchSecret);

    }
}
