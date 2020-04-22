using Data.MSSQL.Common;
using Data.MSSQL.Model.Data;
using House.IService.Common;
using House.IService.Common.Http;
using House.IService.Common.Sign;
using House.IService.Common.XML;
using House.IService.Model;
using House.IService.Model.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace House.Service.Common
{
    public class WeChatPaySingle : IWeChatPaySingle
    {
        private IXMLHelperSingle _Xml;
        private ISignSingle _Sign = null;

        public WeChatPaySingle(IXMLHelperSingle XML, ISignSingle sign)
        {
            this._Xml = XML;
            this._Sign = sign;
        }

        public Dictionary<string,object> GetPrepaySign(wy_wxpay payModel)
        {
            //1.wy_wxpay生成payModel
            Dictionary<string,object> pay = ToWeChatPayModel(payModel);
            //2.排序去重，并将其生成字符串sign
           string XmlPay = this._Xml.DicToXmlStr(pay);
            //3.获取PrepayId
            string ResponseInfo = HttpHelper.PostHttpResponse(CommonFiled.payUrl, XmlPay);
            string ReprepayId = GetPrepayId(ResponseInfo);
            if (!string.IsNullOrEmpty(ReprepayId))
            {
                //4.根据prepayId生成JSAPI请求数据
                var MchSec = CommonFiled.MchSecret(payModel.FEE_TYPES);
                return GetParamStrByPrePayId(payModel.APP_ID, ReprepayId, MchSec);
            }
            return null;
        }

        public Dictionary<string,object> GetParamStrByPrePayId(string appId, string prePayId,string MchSecret)
        {
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params.Add("appId", appId);
            Params.Add("timeStamp", CommonFiled.unixTime10);
            Params.Add("nonceStr", CommonFiled.guid);
            Params.Add("package", $"prepay_id={prePayId}");
            Params.Add("signType", "MD5");
            Params.Add("paySign", this._Sign.WePaySign(Params, MchSecret));
            return Params;
        }

        //1.wy_wxpay生成payModel
        private Dictionary<string,object> ToWeChatPayModel(wy_wxpay wxpay)
        {
            //<sign> 0CB01533B8C1EF103065174F50BCA001 </sign>  
            Dictionary<string, object> Paramters = new Dictionary<string, object>();
            Paramters.Add("appid", CommonFiled.appID);
            Paramters.Add("body", wxpay.REMARK);
            Paramters.Add("mch_id", CommonFiled.MchId(wxpay.FEE_TYPES));
            Paramters.Add("nonce_str", wxpay.ID);
            Paramters.Add("notify_url", CommonFiled.payCallBack);
            Paramters.Add("openid", wxpay.OPEN_ID);
            Paramters.Add("out_trade_no", wxpay.ORDER_ID);
            Paramters.Add("spbill_create_ip", wxpay.USER_IP);
            Paramters.Add("total_fee",wxpay.TOTAL_FEE);
            Paramters.Add("trade_type", "JSAPI");
            var MchSec = CommonFiled.MchSecret(wxpay.FEE_TYPES);
            Paramters.Add("sign", this._Sign.WePaySign(Paramters, MchSec));
            return Paramters;
        }

        //2.将payModel生成Dictionary
        private Dictionary<string, object> ToDictionary<T>(T payModel) where T : class
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();
            Type t = payModel.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(payModel, new Object[] { }));
                }
            }
            return map;
        }

        private string GetPrepayId(string XmlStr)
        {
            bool State = false;
            var wxPay = PayReModel(XmlStr,out State);
            if (State)
            {
                return wxPay["prepay_id"].ToString();
            }
            return null;
        }

        private string SUCCESS = "SUCCESS", FAIL = "FAIL";

        private Dictionary<string,object> PayReModel(string xmlStr,out bool State) {
            //1.解析xml为class
            Dictionary<string,object> wxPay = this._Xml.XmlStrToDic(xmlStr);
            State = false;
            //2.校验Sign
            if (wxPay["return_code"].ToString() == SUCCESS && wxPay["result_code"].ToString() == SUCCESS)
            {
                string Sign = wxPay["sign"].ToString();
                wxPay.Remove("sign");
                var Secret = CommonFiled.MchSecret(wxPay["mch_id"].ToString());
                if (Sign == this._Sign.WePaySign(wxPay, Secret)) {
                    State = true;
                    return wxPay;
                };
            }
            return wxPay; 
        }

        public bool CheckWxSign(Dictionary<string, object> Dic)
        {
            if (!Dic.ContainsKey("mch_id"))
            {
                return false;
            }
            string MchId = CommonFiled.MchSecret(Dic["mch_id"].ToString());
            return this._Sign.CheckSign(Dic, MchId);
        }
    }
}
