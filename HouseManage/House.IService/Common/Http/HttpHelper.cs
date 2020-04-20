using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace House.IService.Common.Http
{
    public class HttpHelper
    {
        /// <summary>
        /// 微信支付接口中，涉及资金回滚的接口会使用到API证书，包括退款、撤销接口
        /// </summary>
        /// <param name="p12Path">证书地址</param>
        /// <param name="password">证书密码默认为您的商户ID</param>
        /// <returns></returns>
        public static X509Certificate getSp12(string p12Path,string password) { 
            return new X509Certificate(p12Path, password);
        }

        /*CheckValidationResult的定义*/
        private static bool CheckValidationResult(object sender,
        X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>  
        /// POST提交
        /// </summary>  
        /// <param name="url">请求地址</param>  
        /// <param name="xmlParam">xml参数</param>  
        /// <returns>返回结果</returns>  
        public static string PostHttpResponse(string url, string xmlParam, X509Certificate x509 = null)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            if (null != x509)
            {
                ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback(CheckValidationResult);
                myHttpWebRequest.ClientCertificates.Add(x509);
            }
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            // Encode the data  
            byte[] encodedBytes = Encoding.UTF8.GetBytes(xmlParam);
            myHttpWebRequest.ContentLength = encodedBytes.Length;
            // Write encoded data into request stream  
            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(encodedBytes, 0, encodedBytes.Length);
            requestStream.Close();
            HttpWebResponse result;
            try
            {
                result = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return string.Empty;
            }
            if (result.StatusCode == HttpStatusCode.OK)
            {
                using (Stream mystream = result.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(mystream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}
