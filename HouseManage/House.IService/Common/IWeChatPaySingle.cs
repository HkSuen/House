using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Common
{
    public interface IWeChatPaySingle
    {
        /// <summary>
        /// 生成统一下单支付接口
        /// 接口返回预付订单Id：preorder_id
        /// </summary>
        /// <returns></returns>
        string CreateOrder();

    }
}
