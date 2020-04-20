using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Model
{
    public class WxPayReModel
    {
        /// <summary>
        /// SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }

        /// <summary>
        /// 当return_code为FAIL时返回信息为错误原因 ，例如 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }

        /// <summary>
        /// 调用接口提交的公众账号ID
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 调用接口提交的商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 微信返回的随机字符串
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 微信返回的签名值
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// 当result_code为FAIL时返回错误代码，详细参见下文错误列表
        /// </summary>
        public string err_code { get; set; }

        /// <summary>
        /// 当result_code为FAIL时返回错误描述，详细参见下文错误列表
        /// </summary>
        public string err_code_des { get; set; }

        /// <summary>
        /// JSAPI -JSAPI支付
        /// NATIVE -Native支付
        ///APP -APP支付
        /// </summary>
        public string trade_type { get; set; }

        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id { get; set; }

        /// <summary>
        /// trade_type=NATIVE时有返回，此url用于生成支付二维码，然后提供给用户进行扫码支付。
        ///注意：code_url的值并非固定，使用时按照URL格式转成二维码即可
        /// </summary>
        public string code_url { get; set; }

    }
}
