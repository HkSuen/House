using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;
using CloudMeet.server;
using plat.orm;
using CloudMeet.orm;
using CloudMeet.orm.WXModel;
using System.Web.Security;

namespace WeiXinPublic
{
    public class WeiXinHelp
    {
        private static readonly string appid = "wx788131db880bb8f4";
        private static readonly string secret = "e873a2859c8e56c83235a7b4cd90160c";

        HttpContext context = HttpContext.Current;

        public static string SendPost(string url, string requestData)
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

        public static string SendGet(string url)
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

        // 验证微信服务器的方法
        public static void CheckServer()
        {
            //1 自己的服务器代码接受微信提交过来的4个参数
            HttpContext context = HttpContext.Current;//得到当前请求的上下文对象
            string token = "wxjhkj";
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            string echostr = context.Request["echostr"];
            string[] temp = { token, timestamp, nonce };
            //字典排序
            Array.Sort(temp);
            //3个参数拼接成一个字符串
            string temp1 = string.Join("", temp);
            //字符串进行sha1加密
            string code = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(temp1, "SHA1");
            if (code.ToLower().Equals(signature))
            {
                //比较一致，表示通过微信的效验了，返回echostr字符串
                context.Response.Write(echostr);
            }

        }

        //获取access_token
        public static string GetAccessToken()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Wx_config Model = Wx_configService.GetWxconfigInfor("履职");
            string tk = "";
            if (Model != null)//更新操作
            {
                if (Model.access_token == "" || Model.TokenDateTime.ToString() == "")
                {
                    //请求微信服务器得到accessToken
                    string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", Model.Appid.ToString(), Model.AppSecret.ToString());
                    string access_token = SendGet(url);
                    AccessToken t = js.Deserialize<AccessToken>(access_token);//反序列化，把JSON字符串变成一个类                
                    tk = t.access_token;//得到调用接口的票据
                    Wx_configService.UpdateWxconfig(tk, "履职");
                }
                else
                {
                    DateTime date1 = Convert.ToDateTime(Model.TokenDateTime.ToString());
                    DateTime date2 = System.DateTime.Now;
                    TimeSpan timeSpan = date2 - date1;
                    if (timeSpan.TotalMinutes > 60)
                    {
                        string url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", Model.Appid.ToString(), Model.AppSecret.ToString());
                        string access_token = SendGet(url);
                        AccessToken t = js.Deserialize<AccessToken>(access_token);//反序列化，把JSON字符串变成一个类                    
                        tk = t.access_token;//得到调用接口的票据
                        Wx_configService.UpdateWxconfig(tk, "履职");
                    }
                    else
                    {
                        tk = Model.access_token.ToString();
                    }
                }
            }
            return tk;
        }

        //获取OpenId
        public static string GetOpenId()
        {
            string openid = "";
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;//获取当前url
                                                                                //if (System.Web.HttpContext.Current.Session["openid"] == "" || System.Web.HttpContext.Current.Session["openid"] == null)
                                                                                //{
                                                                                //先要判断是否是获取code后跳转过来的
            if (System.Web.HttpContext.Current.Request.QueryString["code"] == "" || System.Web.HttpContext.Current.Request.QueryString["code"] == null)
            {
                //Code为空时，先获取Code
                string GetCodeUrls = GetCodeUrl(url);
                System.Web.HttpContext.Current.Response.Redirect(GetCodeUrls);//先跳转到微信的服务器，取得code后会跳回来这页面的

            }
            else
            {
                //Code非空，已经获取了code后跳回来啦，现在重新获取openid
                openid = GetOauthAccessOpenId(System.Web.HttpContext.Current.Request.QueryString["Code"]);//重新取得用户的openid
                                                                                                          //System.Web.HttpContext.Current.Session["openid"] = openid;
            }
            //}
            //else
            //{
            //    openid = System.Web.HttpContext.Current.Session["openid"].ToString();
            //}
            return openid;
        }

        public static string GetCodeUrl(string url)
        {
            string CodeUrl = "";
            //对url进行编码
            url = System.Web.HttpUtility.UrlEncode(url);
            CodeUrl = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + url + "&response_type=code&scope=snsapi_base&state=1#wechat_redirect");
            return CodeUrl;
        }

        public static string GetOauthAccessOpenId(string code)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + code + "&grant_type=authorization_code";
            string access_token = SendGet(url);
            OAuthToken ac = js.Deserialize<OAuthToken>(access_token);
            return ac.openid;
        }

        //jsapi_ticket
        public static string GetHsJsApiTicket(string accessToken)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Wx_config Model = Wx_configService.GetWxconfigInfor("履职");
            string tk = "";
            if (Model != null)//更新操作
            {
                if (Model.jsapi_ticket == "" || Model.jsapi_ticketTime.ToString() == "")
                {
                    var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken);
                    string ticketjson = SendGet(url);
                    JsApiTicket t = js.Deserialize<JsApiTicket>(ticketjson);
                    tk = t.ticket;
                    Wx_configService.UpdateWxTicket(tk,"履职");
                }
                else
                {
                    DateTime date1 = Convert.ToDateTime(Model.jsapi_ticketTime.ToString());
                    DateTime date2 = System.DateTime.Now;
                    TimeSpan timeSpan = date2 - date1;
                    if (timeSpan.TotalMinutes > 60)
                    {
                        var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", accessToken);
                        string ticketjson = SendGet(url);
                        JsApiTicket t = js.Deserialize<JsApiTicket>(ticketjson);
                        tk = t.ticket;
                        Wx_configService.UpdateWxTicket(tk, "履职");
                    }
                    else
                    {
                        tk = Model.jsapi_ticket.ToString();
                    }
                }
            }
            return tk;



        }


        /// <summary>
        /// 获取签名用于JS-SDK的调用
        ///出于安全考虑，开发者必须在服务器端实现签名的逻辑。   
        /// </summary>
        /// <param name="noncestr">必须与wx.config中的nonceStr相同</param>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="timestamp">必须与wx.config中的timestamp相同</param>
        /// <param name="url">url必须是调用JS接口页面的完整URL</param>
        /// <returns></returns>
        public static string GetJsApiSign(string noncestr, string jsapi_ticket, string timestamp, string url)
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
            return FormsAuthentication.HashPasswordForStoringInConfigFile(temp, "SHA1").ToLower();
        }


        /// <summary>
        /// 获取当前请求的url
        /// </summary>
        /// <returns></returns> 
        public static string GetRawUrl()
        {
            return string.Format("http://{0}{1}", GetCurrentFullHost(), HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }

        public static int GetTime()
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return (int)(DateTime.Now - dateStart).TotalSeconds;
        }

        //文本消息
        public void TextMessage(string toUserName, string formUserName, string content)
        {
            string msg = string.Format(@"<xml>
                                        <ToUserName><![CDATA[{0}]]></ToUserName>
                                        <FromUserName><![CDATA[{1}]]></FromUserName>
                                        <CreateTime>{2}</CreateTime>
                                        <MsgType><![CDATA[text]]></MsgType>
                                        <Content><![CDATA[{3}]]></Content>
                                        </xml>", formUserName, toUserName, GetTime(), content);
            context.Response.Write(msg);

        }

        //图文消息
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
            context.Response.Write(msg);
        }

        //音乐链接
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
            context.Response.Write(msg);
        }

    }
}