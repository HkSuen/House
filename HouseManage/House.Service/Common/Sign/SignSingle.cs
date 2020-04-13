using House.IService.Common.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace House.Service.Common.Sign
{
    public class SignSingle : ISignSingle
    {
        public string WePaySign(IDictionary<string, string> InDict, string TenPayV3_Key)
        {
            string[] arrKeys = InDict.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);  //参数名ASCII码从小到大排序；0,A,B,a,b;
            var StrA = new StringBuilder();
            foreach (var key in arrKeys)
            {
                string value = InDict[key];
                if (!String.IsNullOrEmpty(value)) //空值不参与签名
                {
                    StrA.Append(key + "=")
                       .Append(value + "&");
                }
            }
            StrA.Append("key=" + TenPayV3_Key); //注：key为商户平台设置的密钥key
            return Md5(StrA.ToString()).ToUpper();
        }

        public string Md5(string str)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(str);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public string Sha1(string str)
        {
            using (var sha1 = SHA1.Create())
            {
                var result = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToUpper();
            }
        }

    }
}
