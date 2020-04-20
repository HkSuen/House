using Data.MSSQL.Common;
using House.IService.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using AppJson = Data.MSSQL.Common;

namespace House.IService.Common
{
    public class CommonFiled
    {
        private static string GetJson(string key)
        {
            return AppJson.JsonReader.Get(key);
        }

        /// <summary>
        /// 随机标识
        /// </summary>
        public static string guid  => Guid.NewGuid().ToString("N");

        /// <summary>
        /// unix time 10
        /// </summary>
        public static string unixTime10
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public static string orderId => new Random().Next(100, 999)+ DateTime.Now.ToString("yyyyMMddHHmmssffff");

        /// <summary>
        /// 随机出现一个字母
        /// </summary>
        public static char ABC
        {
            get
            {
                char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                int index = new Random().Next(0, Pattern.Length - 1);
                return Pattern[index];
            }
        }


        #region 商户配置
        /// <summary>
        /// app id 
        /// </summary>
        public static string appID => GetJson("WxInfo:AppId");
        /// <summary>
        /// app secret
        /// </summary>
        public static string appSecret => GetJson("WxInfo:AppSecret");
        /// <summary>
        /// 微信对接token
        /// </summary>
        public static string token => GetJson("WxInfo:Token");
        /// <summary>
        /// 支付回调地址
        /// </summary>
        public static string payCallBack => GetJson("WxInfo:Pay:CallBackUrl");
        /// <summary>
        /// 支付Url
        /// </summary>
        public static string payUrl => GetJson("WxInfo:Pay:Pay_url");

        /// <summary>
        /// 缴费分类的枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FeeTypes EnumFeeTypes(int type)
        {
            return((FeeTypes)type);
        }

        /// <summary>
        /// 获取商户ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MchId(int type)
        {
            string typeStr = ((int)EnumFeeTypes(type)).ToString();
            return MchId(typeStr);
        }

        /// <summary>
        /// 获取商户ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MchId(FeeTypes type)
        {
            string typeStr = ((int)type).ToString();
            return MchId(typeStr);
        }
        /// <summary>
        /// 获取商户ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MchId(string type)
        {
            return dicInfo("WxInfo:Pay:MchID")[type];
        }

        /// <summary>
        /// 商户秘钥
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static string MchSecret(int type)
        {
            string mId = MchId(type);
            return dicInfo("WxInfo:Pay:Secret")[mId];
        }

        /// <summary>
        /// 商户秘钥
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static string MchSecret(FeeTypes type)
        {
            string mId = MchId(type);
            return dicInfo("WxInfo:Pay:Secret")[mId];
        }

        /// <summary>
        /// 商户秘钥
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static string MchSecret(string mchId)
        {
            return dicInfo("WxInfo:Pay:Secret")[mchId];
        }
        /// <summary>
        /// 商户名字
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public static string MchName(string mchId)
        {
            return dicInfo("WxInfo:Pay:MchName")[mchId];
        }
        private static Dictionary<string,string> dicInfo(string key)
        {
            string value = GetJson(key);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }

        #endregion

        #region 交易类型
        public static string JSAPI = "JSAPI";
        public static string NATIVE = "NATIVE";
        public static string APP = "APP";
        #endregion
    }
}
