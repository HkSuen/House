using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace House.IService.Model
{
    public class WeChatPayModel
    {
        /// <summary>
        /// 公众账号ID
        /// 微信支付分配的公众账号ID（企业号corpid即为此appId）
        /// </summary>
        [Required]
        public string appid { get; set; }
        /// <summary>
        /// 商户号
        /// 微信支付分配的商户号
        /// </summary>
        [Required]
        public string mch_id { get; set; }
        /// <summary>
        /// 随机字符串
        /// 随机字符串，长度要求在32位以内。推荐随机数生成算法
        /// </summary>
        [Required]
        public string nonce_str => Guid.NewGuid().ToString("N");
        /// <summary>
        /// 签名
        /// 通过签名算法计算得出的签名值，详见签名生成算法
        /// </summary>
        [Required]
        public string sign { get; set; }
        /// <summary>
        /// 商品描述
        /// 商品简单描述，该字段请按照规范传递，具体请见参数规定
        /// </summary>
        [Required]
        public string body { get; set; }
        /// <summary>
        /// 商户订单号
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|* 且在同一个商户号下唯一。详见商户订单号
        /// </summary>
        [Required]
        public string out_trade_no { get; set; }
        /// <summary>
        /// 标价金额
        /// 订单总金额，单位为分，详见支付金额
        /// </summary>
        [BindRequired]
        public int total_fee { get; set; }
        /// <summary>
        /// 终端IP
        /// </summary>
        [Required]
        public string spbill_create_ip { get; set; }
        /// <summary>
        /// 通知地址
        /// 异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。
        /// </summary>
        [Required]
        public string notify_url { get; set; }
        /// <summary>
        /// 交易类型
        /// JSAPI -JSAPI支付
        /// NATIVE -Native支付
        /// APP -APP支付
        /// </summary>
        [Required]
        public string trade_type { get; set; }
        [Required]
        public string open_id { get; set; }

    }
}
