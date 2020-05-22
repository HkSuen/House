using House.IService.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace House.IService.Common
{
    public interface IWeiXinSingle
    {
        #region 请求方式
        string SendPost(string url, string requestData);
        string SendGet(string url);

        void SetSession(string key, string val);
        string GetSession(string key);

        #endregion

        /// <summary>
        /// 验证微信服务器的方法
        /// </summary>
        Task<string> CheckServer();

        /// <summary>
        /// 获取普通的 access_token的方式（直接使用appid和AppSecret的方式） 有效期两小时
        /// </summary>
        /// <returns></returns>
        AccessToken GetAccessToken();

        /// <summary>
        /// 获取openId的方法
        /// </summary>
        /// <returns></returns>
        string GetOpenId();

        /// <summary>
        /// 获取Code的地址(地址回调使用的)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetCodeUrl(string url);

        /// <summary>
        /// 通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        OAuthToken GetOauthAccessOpenId(string code);

        //以下3步骤，有待添加，但是目前的开发应用不到。
        //刷新access_token（如果需要）
        //拉取用户信息(需scope为 snsapi_userinfo)
        //检验授权凭证（access_token）是否有效


        //jsapi_ticket
        JsApiTicket GetHsJsApiTicket(string accessToken);

        /// <summary>
        /// 获取签名用于JS-SDK的调用
        ///出于安全考虑，开发者必须在服务器端实现签名的逻辑。   
        /// </summary>
        /// <param name="noncestr">必须与wx.config中的nonceStr相同</param>
        /// <param name="jsapi_ticket">jsapi_ticket</param>
        /// <param name="timestamp">必须与wx.config中的timestamp相同</param>
        /// <param name="url">url必须是调用JS接口页面的完整URL</param>
        /// <returns></returns>
        string GetJsApiSign(string noncestr, string jsapi_ticket, string timestamp, string url);

        /// <summary>
        /// 获取当前请求的url
        /// </summary>
        /// <returns></returns> 
        string GetRawUrl();

        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        string GetCurrentFullHost();

        ///UnixTime;
        int GetTime();

        /// <summary>
        /// 文本消息
        /// </summary>
        /// <param name="toUserName"></param>
        /// <param name="formUserName"></param>
        /// <param name="content"></param>
        void TextMessage(string toUserName, string formUserName, string content);

        /// <summary>
        /// 图文消息
        /// </summary>
        /// <param name="tosuerName"></param>
        /// <param name="fromUserName"></param>
        /// <param name="dt"></param>
        void NewsMessage(string tosuerName, string fromUserName, System.Data.DataTable dt);

        /// <summary>
        /// 音乐消息
        /// </summary>
        /// <param name="touserName"></param>
        /// <param name="formUserName"></param>
        /// <param name="title"></param>
        /// <param name="remark"></param>
        /// <param name="url"></param>
        void MusicMessage(string touserName, string formUserName, string title, string remark, string url);

        object JsApiSignature(string requestUrl);
    }
}
