using Data.MSSQL.Common;
using House.IService.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// 支付查询订单二次校验地址。
        /// </summary>
        public static string findOrdersUrl => GetJson("WxInfo:Pay:FindOrderUrl");

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

        public static string FeeTypeName(int type)
        {
            return Fee.Types.Where(c => c.Value == type).FirstOrDefault().Key;
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
            return MchNameInfo[mchId];
        }
        public static string MchName(int type)
        {
            string mId = MchId(type);
            return MchName(mId);
        }
        public static string MchName(FeeTypes type)
        {
            string mId = MchId(type);
            return MchName(mId);
        }

        public static Dictionary<int, string> TypesAndMechName {
            get
            {
                var Infos = new Dictionary<int, string>();
                foreach(var type in Fee.Types)
                {
                    var value = type.Value;
                    Infos.Add(value, MchName(value));
                }
                return Infos;
            }
        }

        private static Dictionary<string, string> MchNameInfo {
            get
            {
                var Infos = new Dictionary<string, string>();
                Infos.Add("1584864331", "普丰物业分公司");
                Infos.Add("1585873541", "普丰房地产分公司");
                return Infos;
            }
        }
        private static Dictionary<string,string> dicInfo(string key)
        {
            string value = GetJson(key);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
        }

        #endregion

        public static string FAIL = "FAIL";
        public static string SUCCESS = "SUCCESS";

        #region 交易类型
        public static string JSAPI = "JSAPI";
        public static string NATIVE = "NATIVE";
        public static string APP = "APP";
        #endregion

        public static string DomainURL => GetJson("WxInfo:Domain_Url");
        public static string RegisterUrl => GetJson("WxInfo:RegPage");

        /// <summary>
        /// 数字转大写
        /// </summary>
        /// <param name="Num">数字</param>
        /// <returns></returns>

        public static string CmycurD(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上 
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整” 
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

        public static string UnitPriceWaterKey = "PER_WATER_PRICE";
        public static string WX_SMS_STATUS = "SMS_STATUS";
        public static string WX_MSG_STATUS = "WX_MSG_STATUS";

        public static string MsgPayUrl => AppSetting.AppSetting.GetSection("msgUrl:pay");
        public static string MsgUrl => AppSetting.AppSetting.GetSection("msgUrl:url");
        public static string MsgTempId => AppSetting.AppSetting.GetSection("msgtemp:temp");
        /// <summary>
        /// 短信的地址
        /// </summary>
        public static string MsgSMSUrl => AppSetting.AppSetting.GetSection("msgUrl:sms");
        /// <summary>
        /// 短信模板ID：//短信模板ID：【港西新城】{1}您收到一个{2},详情请登陆微信公众号查看
        /// </summary>
        public static string MsgSMSTemp => AppSetting.AppSetting.GetSection("msgsmstemp:smstemp");

    }
}
