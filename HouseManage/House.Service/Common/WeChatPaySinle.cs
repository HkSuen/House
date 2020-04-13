using House.IService.Common;
using House.IService.Common.Sign;
using House.IService.Common.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace House.Service.Common
{
    public class WeChatPaySinle : IWeChatPaySingle
    {
        private readonly string _PayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        private readonly string _PayUrlCopy = "https://api2.mch.weixin.qq.com/pay/unifiedorder";

        private IXMLHelperSingle _Xml;
        private ISignSingle _Sign = null;
        public WeChatPaySinle(IXMLHelperSingle XML,ISignSingle sign)
        {
            this._Xml = XML;
            this._Sign = sign;
        }


        public string CreateOrder()
        {
            return null;
        }

    }
}
