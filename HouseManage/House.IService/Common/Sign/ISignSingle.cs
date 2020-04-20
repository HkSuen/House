using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Common.Sign
{
    public interface ISignSingle
    {
        /// <summary>
        /// SHA1签名算法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string Sha1(string str);

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string Md5(string str);

        /// <summary>
        /// 微信支付MD5签名算法，ASCII码字典序排序0,A,B,a,b
        /// </summary>
        /// <param name="InDict">待签名名键值对</param>
        /// <param name="TenPayV3_Key">用于签名的Key,商户平台设置的密钥key</param>
        /// <returns>MD5签名字符串</returns>
        string WePaySign(IDictionary<string, object> InDict, string TenPayV3_Key);
    }
}
