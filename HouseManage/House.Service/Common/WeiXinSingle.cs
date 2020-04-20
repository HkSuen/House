using House.IService.Common;
using House.IService.Common.Sign;
using House.IService.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Config = Data.MSSQL.Common;

namespace House.Service.Common
{
    /// <summary>
    /// 后续常量规范化，待修改[标记]
    /// </summary>
    public class WinXinSingle : IWeiXinSingle
    {
        private IHttpContextAccessor _httpContext = null;
        private ISignSingle _sign = null;
        private string AppId => Config.JsonReader.Get("WxInfo:AppId");
        private string AppSecret => Config.JsonReader.Get("WxInfo:AppSecret");

        public WinXinSingle(IHttpContextAccessor http, ISignSingle sign)
        {
            this._httpContext = http;
            this._sign = sign;
        }

        #region Private 
        private Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return _httpContext.HttpContext;
            }
        }

        private string ParamsQuery(string param)
        {
            return this._httpContext.HttpContext.Request.Query[param].FirstOrDefault();
        }

        #region Session 操作
        public string GetSession(string key)
        {
            byte[] value = null;
            if (this._httpContext.HttpContext.Session.TryGetValue(key, out value))
            {
                return System.Text.Encoding.Default.GetString(value);
            }
            return null;
        }

        /// <summary>
        /// 已经修改其他的授权方式，Session已经不需要了。暂时弃用
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        [Obsolete]
        public void SetSession(string key, string val)
        {
            byte[] value = System.Text.Encoding.Default.GetBytes(val);
            this._httpContext.HttpContext.Session.Set(key, value);
        }
        #endregion

        #endregion


        public async Task<string> CheckServer()
        {
            //1 自己的服务器代码接受微信提交过来的4个参数
            string token = Config.JsonReader.Get("WxInfo:Token");
            string signature = ParamsQuery("signature");
            string timestamp = ParamsQuery("timestamp");
            string nonce = ParamsQuery("nonce");
            string echostr = ParamsQuery("echostr");
            string[] temp = { token, timestamp, nonce };
            //字典排序
            Array.Sort(temp);
            //3个参数拼接成一个字符串
            string temp1 = string.Join("", temp);
            //字符串进行sha1加密
            string code = this._sign.Sha1(temp1);
            if (code.ToLower().Equals(signature))
            {
                //比较一致，表示通过微信的效验了，返回echostr字符串
                return echostr;
            }
            else
            {
                return "Authorization failed:" + echostr;
            }
        }

        /// <summary>
        /// 获取普通的access_token
        /// </summary>
        /// <returns></returns>
        public AccessToken GetAccessToken()
        {
            //请求微信服务器得到accessToken
            string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                AppId, AppSecret);
            string access_token = SendGet(url);
            return JsonConvert.DeserializeObject<AccessToken>(access_token);
        }

        public string GetCodeUrl(string url)
        {
            //对url进行编码
            url = System.Web.HttpUtility.UrlEncode(url);
            string CodeUrl = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + AppId
                  + "&redirect_uri=" + url + "&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect");
            return CodeUrl;
        }

        public string GetCurrentFullHost() => _httpContext.HttpContext.Request.Host.Value;

        public string GetOpenId()
        {
            string openid = "";
            var host1 = _httpContext.HttpContext.Request.Host.Value;
            var host = "http://1402b84a.ngrok.io"; //测试先写死
            string url = host + _httpContext.HttpContext.Request.Path.Value;
            //先要判断是否是获取code后跳转过来的
            string code = ParamsQuery("code");
            if (string.IsNullOrEmpty(code))
            {
                //Code为空时，先获取Code
                string GetCodeUrls = GetCodeUrl(url);
                _httpContext.HttpContext.Response.Redirect(GetCodeUrls);//先跳转到微信的服务器，取得code后会跳回来这页面的
            }
            else
            {
                string Code = ParamsQuery("code");
                openid = GetOauthAccessOpenId(Code)?.openid;//重新取得用户的openid
                                                            //SetSession("OpenId", Code);
            }
            return openid;
        }

        /// <summary>
        /// 通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public OAuthToken GetOauthAccessOpenId(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + AppId + "&secret=" + AppSecret + "&code=" + code + "&grant_type=authorization_code";
            string access_token = SendGet(url);
            OAuthToken ac = JsonConvert.DeserializeObject<OAuthToken>(access_token);
            return ac;
        }


        public string GetRawUrl()
        {
            return string.Format("http://{0}{1}", GetCurrentFullHost(), _httpContext.HttpContext.Request.PathBase);
        }

        public int GetTime()
        {

            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return (int)(DateTime.Now - dateStart).TotalSeconds;
        }

        public void MusicMessage(string touserName, string formUserName, string title, string remark, string url)
        {
            string msg = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName>
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[music]]></MsgType>
                                        <Music>
                                        <Title><![CDATA[{3}]]></Title>
                                        <Description><![CDATA[{4}]]></Description>
                                        <MusicUrl><![CDATA[{5}]]></MusicUrl>
                                        <HQMusicUrl><![CDATA[{5}]]></HQMusicUrl>
                                        </Music>
                                        </xml>", formUserName, touserName, GetTime(), title, remark, url);
            _httpContext.HttpContext.Response.WriteAsync(msg); // 如果需要同步这里再修改
        }

        public void NewsMessage(string tosuerName, string fromUserName, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.AppendFormat(@"<item>
                                        <Title><![CDATA[{0}]]></Title>
                                        <Description><![CDATA[{1}]]></Description>
                                        <PicUrl><![CDATA[{2}]]></PicUrl>
                                        <Url><![CDATA[{3}]]></Url>
                                        </item>", dt.Rows[i]["Title"].ToString(), dt.Rows[i]["Remark"].ToString(), dt.Rows[i]["PicUrl"].ToString(), dt.Rows[i]["Url"].ToString());
            }
            string msg = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName>
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[news]]></MsgType>
                                        <ArticleCount>{4}</ArticleCount>
                                        <Articles>
                                          {3}
                                        </Articles>
                                        </xml> ", fromUserName, tosuerName, GetTime(), sb.ToString(), dt.Rows.Count);
            _httpContext.HttpContext.Response.WriteAsync(msg);
        }

        public string SendGet(string url)
        {
            //模拟一个浏览器的请求
            //1.创建一个请求对象
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "applicatoin/x-www/form-urlencoded";
            string str = null;//保存请求服务器以后返回的结果
            //得到 响应的内容
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        str = reader.ReadToEnd();
                    }
                }
            }
            return str;
        }

        public string SendPost(string url, string requestData)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            byte[] postDatas = null;
            request.ContentType = "application/x-www-form-urlencoded";
            postDatas = Encoding.UTF8.GetBytes(requestData);
            request.ContentLength = postDatas.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postDatas, 0, postDatas.Length);
            }
            string responseData = null;//服务器响应的数据
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                using (Stream st = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(st, Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }
            return responseData;
        }

        public void TextMessage(string toUserName, string formUserName, string content)
        {
            string msg = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName>
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[text]]></MsgType>
                                        <Content><![CDATA[{3}]]></Content>
                                        </xml>", formUserName, toUserName, GetTime(), content);
            _httpContext.HttpContext.Response.WriteAsync(msg);
        }

        /// <summary>
        /// 获取签名用于JS-SDK的调用
        ///出于安全考虑，开发者必须在服务器端实现签名的逻辑。   
        /// </summary>
        public string GetJsApiSign(string noncestr, string jsapi_ticket, string timestamp, string url)
        {
            //将字段添加到列表中。
            string[] arr = new[]
            {
                string.Format("noncestr={0}",noncestr),
                string.Format("jsapi_ticket={0}",jsapi_ticket),
                string.Format("timestamp={0}",timestamp),
                string.Format("url={0}",url)
            };
            //字典排序
            Array.Sort(arr);
            //使用URL键值对的格式拼接成字符串
            var temp = string.Join("&", arr);
            return this._sign.Sha1(temp).ToLower();
        }

        public JsApiTicket GetHsJsApiTicket(string accessToken)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken);
            string ticketjson = SendGet(url);
            return JsonConvert.DeserializeObject<JsApiTicket>(ticketjson);
        }
    }
}
